using System;
using System.Messaging;

namespace Rhino.ServiceBus.Msmq
{
	using System.Collections.Generic;

	/// <summary>
	/// Handles message moving to sibling queues.
	/// Suitable for MSMQ 3.0
	/// </summary>
	/// <remarks>
	/// <para>
	/// The endpoint is treated as the <c>subscriptions</c> queue.
	/// This strategy presumes two additional queues than those defined by the endpoint.
	/// </para>
	/// <list type="bullet">
	/// <listheader>So your queue structure would be:</listheader>
	/// <item>[my_queue_name]</item>
	/// <item>[my_queue_name]<c>#errors</c></item>
	/// <item>[my_queue_name]<c>#discarded</c></item>
	/// </list>
	/// </remarks>
	public class FlatQueueStrategy : IQueueStrategy
	{
		private readonly Uri endpoint;

		/// <summary>
		/// Initializes a new instance of the <see cref="FlatQueueStrategy"/> class.
		/// </summary>
		/// <param name="endpoint">The endpoint.</param>
		public FlatQueueStrategy(Uri endpoint)
		{
			this.endpoint = endpoint;
		}

		/// <summary>
		/// Creates the subscription queue URI.
		/// </summary>
		/// <param name="subscriptionQueue">The subscription queue.</param>
		/// <returns></returns>
		public Uri CreateSubscriptionQueueUri(Uri subscriptionQueue)
		{
			return subscriptionQueue;
		}

		/// <summary>
		/// Moves the <paramref name="message"/> to subscription queue.
		/// </summary>
		/// <param name="queue">The queue.</param>
		/// <param name="message">The message.</param>
		public void MoveToSubscriptionQueue(MessageQueue queue, Message message)
		{
			//the endpoint IS the subscription queue
			return;
		}

		/// <summary>
		/// Moves the <paramref name="message"/> to errors queue.
		/// </summary>
		/// <param name="queue">The queue.</param>
		/// <param name="message">The message.</param>
		public void MoveToErrorsQueue(MessageQueue queue, Message message)
		{
			
			using (var destinationQueue = new MessageQueue(GetErrorsQueuePath(),QueueAccessMode.Send))
			{
				destinationQueue.Send(queue.ReceiveByLookupId(message.LookupId),
									  queue.GetTransactionType());
			}
		}


		/// <summary>
		/// Moves the <paramref name="message"/> to discarded queue.
		/// </summary>
		/// <param name="queue">The queue.</param>
		/// <param name="message">The message.</param>
		public void MoveToDiscardedQueue(MessageQueue queue, Message message)
		{
			using (var destinationQueue = new MessageQueue(GetDiscardedQueuePath(),QueueAccessMode.Send))
			{
				destinationQueue.Send(queue.ReceiveByLookupId(message.LookupId), destinationQueue.GetTransactionType());
			}
		}

		/// <summary>
		/// Moves the <paramref name="message"/> to the timeout queue.
		/// </summary>
		/// <param name="queue">The queue.</param>
		/// <param name="message">The message.</param>
		public void MoveToTimeoutQueue(MessageQueue queue, Message message)
		{
			using (var destinationQueue = new MessageQueue(GetTimeoutQueuePath(),QueueAccessMode.Send))
			{
				destinationQueue.Send(queue.ReceiveByLookupId(message.LookupId), destinationQueue.GetTransactionType());
			}
		}

		/// <summary>
		/// Gets a listing of all timeout messages.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TimeoutInfo> GetTimeoutMessages(MessageQueue queue)
		{
			yield break;
		}

		/// <summary>
		/// Moves the message from the timeout queue to the main queue.
		/// </summary>
		/// <param name="queue">The queue.</param>
		/// <param name="messageId">The message id.</param>
		public void MoveTimeoutToMainQueue(MessageQueue queue, string messageId)
		{
			using (var destinationQueue = new MessageQueue(GetTimeoutQueuePath(), QueueAccessMode.Receive))
			{
				var message = destinationQueue.ReceiveById(messageId, queue.GetTransactionType());
				message.AppSpecific = 0;//reset timeout flag
				queue.Send(message, destinationQueue.GetTransactionType());
			}
		}

		internal static void InitializeSubQueues(Uri endpoint, bool transactional)
		{
			var siblingQueues = new[] {"#errors", "#discarded", "#timeout"};
			foreach(var sibling in siblingQueues)
			{
				new Uri(endpoint.ToString() + sibling).CreateQueue(transactional,QueueAccessMode.Peek).Close();
			}
		}

		/// <summary>
		/// Gets the errors queue path.
		/// </summary>
		/// <returns></returns>
		private string GetErrorsQueuePath()
		{
			var path = MsmqUtil.GetQueuePath(endpoint);
			return path + "#errors";
		}

		/// <summary>
		/// Gets the discarded queue path.
		/// </summary>
		/// <returns></returns>
		private string GetDiscardedQueuePath()
		{
			var path = MsmqUtil.GetQueuePath(endpoint);
			return path + "#discarded";
		}

		/// <summary>
		/// Gets the timeout queue path.
		/// </summary>
		/// <returns></returns>
		private string GetTimeoutQueuePath()
		{
			var path = MsmqUtil.GetQueuePath(endpoint);
			return path + "#timeout";
		}

	}
}