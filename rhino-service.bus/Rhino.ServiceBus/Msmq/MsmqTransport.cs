using System;
using System.Linq;
using System.Messaging;
using System.Threading;
using System.Transactions;
using log4net;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Msmq.TransportActions;

namespace Rhino.ServiceBus.Msmq
{
	public class MsmqTransport : IMsmqTrasport
	{
		[ThreadStatic]
		private static MsmqCurrentMessageInformation currentMessageInformation;

		private readonly Uri endpoint;
		private readonly ILog logger = LogManager.GetLogger(typeof(MsmqTransport));
		private readonly IMessageSerializer serializer;
		private readonly int threadCount;
		private readonly WaitHandle[] waitHandles;
		private IInitializeSubQueues subQueueInitializer;

		private bool haveStarted;
		
		private MessageQueue queue;
	    private readonly IMessageAction[] messageActions;

		public MsmqTransport(
			IMessageSerializer serializer,
			Uri endpoint,
			int threadCount,
            IMessageAction[] messageActions)
		{
			this.serializer = serializer;
		    this.messageActions = messageActions;
		    this.endpoint = endpoint;
			this.threadCount = threadCount;
			waitHandles = new WaitHandle[threadCount];
		}

		public IInitializeSubQueues SubQueueInitializer
		{
			get { return subQueueInitializer; }
			set { this.subQueueInitializer = value;}
		}

		public volatile bool ShouldStop;

		#region ITransport Members

		public Uri Endpoint
		{
			get { return endpoint; }
		}

		public void Start()
		{
			if (haveStarted)
				return;

			logger.DebugFormat("Starting msmq transport on: {0}", Endpoint);
			queue = InitalizeQueue(endpoint);
			
			if(subQueueInitializer != null)
			{
				subQueueInitializer.InitializeSubQueues(Endpoint,queue.Transactional);
			}

		    foreach (var messageAction in messageActions)
		    {
		        messageAction.Init(this);
		    }

			for (var t = 0; t < threadCount; t++)
			{
				var waitHandle = new ManualResetEvent(true);
				waitHandles[t] = waitHandle;
				try
				{
					queue.BeginPeek(TimeOutForPeek, new QueueState
					{
						Queue = queue,
						WaitHandle = waitHandle
					}, OnPeekMessage);
					waitHandle.Reset();
				}
				catch (Exception e)
				{
					throw new TransportException("Unable to start reading from queue: " + endpoint, e);
				}
			}

			haveStarted = true;
		}

		private static TimeSpan TimeOutForPeek
		{
			get { return TimeSpan.FromHours(1); }
		}

		public void Stop()
		{
			ShouldStop = true;
			queue.Send(new Message
			{
				Label = "Shutdown bus, if you please",
				AppSpecific = (int)MessageType.ShutDownMessageMarker
			}, queue.GetSingleMessageTransactionType());

			WaitForProcessingToEnd();

			if (queue != null)
				queue.Close();
			haveStarted = false;
		}

		public void Reply(params object[] messages)
		{
			if (currentMessageInformation == null)
				throw new TransactionException("There is no message to reply to, sorry.");

			Send(currentMessageInformation.Source, messages);
		}

		public event Action<CurrentMessageInformation> MessageSent;
		public event Func<CurrentMessageInformation,bool> AdministrativeMessageArrived;
		public event Action<CurrentMessageInformation> MessageArrived;
		public event Action<CurrentMessageInformation, Exception> MessageProcessingFailure;
        public event Action<CurrentMessageInformation, Exception> MessageProcessingCompleted;
        public event Action<CurrentMessageInformation, Exception> AdministrativeMessageProcessingCompleted;

		public void Discard(object msg)
		{
			var message = GenerateMsmqMessageFromMessageBatch(new[] { msg });

			message.AppSpecific = (int)MessageType.DiscardedMessageMarker;

			SendMessageToQueue(message, Endpoint);
		}

	    public bool RaiseAdministrativeMessageArrived(CurrentMessageInformation information)
	    {
            var copy = AdministrativeMessageArrived;
            if (copy != null)
                return copy(information);
	        return false;
        }

	    public MessageQueue Queue
	    {
	        get { return queue; }
	    }

	    public void RaiseAdministrativeMessageProcessingCompleted(CurrentMessageInformation information, Exception ex)
	    {
	        var copy = AdministrativeMessageProcessingCompleted;
            if (copy != null)
                copy(information, ex);
	    }

	    public void Send(Uri uri, DateTime processAgainAt, object[] msgs)
		{
			var message = GenerateMsmqMessageFromMessageBatch(msgs);

			message.Extension = BitConverter.GetBytes(processAgainAt.ToBinary());
			message.AppSpecific = (int)MessageType.TimeoutMessageMarker;

			SendMessageToQueue(message, uri);
		}

		public void Send(Uri uri, params object[] msgs)
		{
			var message = GenerateMsmqMessageFromMessageBatch(msgs);

			SendMessageToQueue(message, uri);

			var copy = MessageSent;
			if (copy == null)
				return;

			copy(new CurrentMessageInformation
			{
				AllMessages = msgs,
				Source = endpoint,
				Destination = uri,
				CorrelationId = CorrelationId.Parse(message.CorrelationId),
				MessageId = CorrelationId.Parse(message.Id),
			});
		}

		private Message GenerateMsmqMessageFromMessageBatch(object[] msgs)
		{
			var message = new Message();

			serializer.Serialize(msgs, message.BodyStream);

			message.ResponseQueue = queue;

			SetCorrelationIdOnMessage(message);

			message.AppSpecific =
				msgs[0] is AdministrativeMessage ? (int)MessageType.AdministrativeMessageMarker : 0;

			message.Label = msgs
				.Where(msg => msg != null)
				.Select(msg =>
				{
					string s = msg.ToString();
					if (s.Length > 249)
						return s.Substring(0, 246) + "...";
					return s;
				})
				.FirstOrDefault();
			return message;
		}

		public event Action<CurrentMessageInformation, Exception> MessageSerializationException;

		#endregion

		private void WaitForProcessingToEnd()
		{
			if (haveStarted == false)
				return;

			if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
			{
				WaitHandle.WaitAll(waitHandles);
			}
			else
			{
				foreach (WaitHandle handle in waitHandles)
				{
					if (handle != null)
						handle.WaitOne();
				}
			}
		}

		private static MessageQueue InitalizeQueue(Uri endpoint)
		{
		    try
			{
					
				var messageQueue = endpoint.CreateQueue(QueueAccessMode.SendAndReceive);
				var filter = new MessagePropertyFilter();
				filter.SetAll();
				messageQueue.MessageReadPropertyFilter = filter;
				return messageQueue;
			}
			catch (Exception e)
			{
				throw new TransportException(
					"Could not open queue: " + endpoint + Environment.NewLine +
					"Queue path: " + MsmqUtil.GetQueuePath(endpoint), e);
			}
		}

		private void OnPeekMessage(IAsyncResult ar)
		{
			Message message;
			bool? peek = TryEndingPeek(ar, out message);
			if (peek == false) // error 
				return;

			var state = (QueueState)ar.AsyncState;
			if (ShouldStop)
			{
				state.WaitHandle.Set();
				return;
			}

			try
			{
				if (peek == null)//nothing was found 
					return;

                logger.DebugFormat("Got message {0} from {1}",
                                  message.Label,
                                  MsmqUtil.GetQueueUri(state.Queue));

			    foreach (var action in messageActions)
			    {
			        if (action.CanHandlePeekedMessage(message) == false) 
                        continue;

			        if(action.HandlePeekedMessage(queue, message))
			            return;
			    }
			   
				ReceiveMessageInTransaction(state, message.Id);

			}
			finally
			{
				state.Queue.BeginPeek(TimeOutForPeek, state, OnPeekMessage);
			}
		}

		private void ReceiveMessageInTransaction(QueueState state, string messageId)
		{
			using (var tx = new TransactionScope())
			{
				Message message = state.Queue.TryGetMessageFromQueue(messageId);
                
                if (message == null)
                    return;// someone else got our message, better luck next time

                ProcessMessage(message, state.Queue, tx, MessageArrived, MessageProcessingCompleted);
			}
		}

		private void HandleMessageCompletion(
			Message message,
			TransactionScope tx,
            MessageQueue messageQueue,
			Exception exception)
		{
			if (exception == null)
			{
				try
				{
					if (tx != null)
						tx.Complete();
					return;
				}
				catch (Exception e)
				{
					logger.Warn("Failed to complete transaction, moving to error mode", e);
				}
			}
			if (message == null)
				return;

            try
            {
                Action<CurrentMessageInformation, Exception> copy = MessageProcessingFailure;
                if (copy != null)
                    copy(currentMessageInformation, exception);
            }
            catch (Exception moduleException)
            {
                string exMsg = "";
                if (exception != null)
                    exMsg = exception.Message;
                logger.Error("Module failed to process message failure: " + exMsg,
                                             moduleException);
            }

            if (messageQueue.Transactional == false)// put the item back in the queue
			{
                messageQueue.Send(message, MessageQueueTransactionType.None);
			}
		}

		private bool? TryEndingPeek(IAsyncResult ar, out Message message)
		{
			var state = (QueueState)ar.AsyncState;
			try
			{
				message = state.Queue.EndPeek(ar);
			}
			catch (MessageQueueException e)
			{
				message = null;
				if (e.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
				{
					logger.Error("Could not peek message from queue", e);
					return false;
				}
				return null; // nothing found
			}
			return true;
		}

	    public void ProcessMessage(Message message, 
            MessageQueue messageQueue, 
            TransactionScope tx,
            Action<CurrentMessageInformation> messageRecieved,
            Action<CurrentMessageInformation, Exception> messageCompleted)
		{
		    Exception ex = null;
		    currentMessageInformation = CreateMessageInformation(message, null, null);
            try
            {
                //deserialization errors do not count for module events
                object[] messages = DeserializeMessages(messageQueue, message);
                try
                {
                    foreach (object msg in messages)
                    {
                        currentMessageInformation = CreateMessageInformation(message, messages, msg);

                        if (messageRecieved != null)
                            messageRecieved(currentMessageInformation);
                    }
                }
                catch (Exception e)
                {
                    ex = e;
                    logger.Error("Failed to process message", e);
                }
                finally
                {
                    if (messageCompleted != null)
                        messageCompleted(currentMessageInformation, ex);
                }
            }
            catch (Exception e)
            {
                ex = e;
                logger.Error("Failed to deserialize message", e);
            }
            finally
		    {
                HandleMessageCompletion(message, tx, messageQueue, ex);
                currentMessageInformation = null;
		    } 
		}

	    private MsmqCurrentMessageInformation CreateMessageInformation(Message message, object[] messages, object msg)
	    {
	        return new MsmqCurrentMessageInformation
	        {
	            MessageId = CorrelationId.Parse(message.Id),
	            AllMessages = messages,
	            CorrelationId = CorrelationId.Parse(message.CorrelationId),
	            Message = msg,
	            Queue = queue,
	            Destination = Endpoint,
	            Source = MsmqUtil.GetQueueUri(message.ResponseQueue),
	            MsmqMessage = message,
	            TransactionType = queue.GetTransactionType()
	        };
	    }

        private object[] DeserializeMessages(MessageQueue messageQueue, Message transportMessage)
		{
			object[] messages;
			try
			{
				messages = serializer.Deserialize(transportMessage.BodyStream);
			}
			catch (Exception e)
			{
				try
				{
					logger.Error("Error when serializing message", e);
					Action<CurrentMessageInformation, Exception> copy = MessageSerializationException;
					if (copy != null)
					{
						var information = new MsmqCurrentMessageInformation
						{
                            MsmqMessage = transportMessage,
                            Queue = messageQueue,
                            CorrelationId = CorrelationId.Parse(transportMessage.CorrelationId),
							Message = transportMessage,
                            Source = MsmqUtil.GetQueueUri(messageQueue),
							MessageId = CorrelationId.Parse(transportMessage.Id)
						};
						copy(information, e);
					}
				}
				catch (Exception moduleEx)
				{
					logger.Error("Error when notifying about serialization exception", moduleEx);
				}
				throw;
			}
			return messages;
		}

		private static void SetCorrelationIdOnMessage(Message message)
		{
		    if (currentMessageInformation == null) 
                return;

		    message.CorrelationId = currentMessageInformation
		        .CorrelationId.Increment().ToString();
		}

	    private void SendMessageToQueue(Message message, Uri uri)
		{
			if (haveStarted == false)
				throw new TransportException("Cannot send message before transport is started");

			string sendQueueDescription = MsmqUtil.GetQueuePath(uri);
			try
			{
				using (var sendQueue = new MessageQueue(
					sendQueueDescription,
					QueueAccessMode.Send))
				{
					MessageQueueTransactionType transactionType = sendQueue.GetTransactionType();
					sendQueue.Send(message, transactionType);
					logger.DebugFormat("Send message {0} to {1}", message.Label, uri);
				}
			}
			catch (Exception e)
			{
				throw new TransactionException("Failed to send message to " + uri, e);
			}
		}
	}
}
