using System;
using System.Messaging;
using log4net;
using Rhino.ServiceBus.DataStructures;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;

namespace Rhino.ServiceBus.Msmq.TransportActions
{
    public class ErrorAction : IMessageAction
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(ErrorAction));
        private readonly int numberOfRetries;
        private readonly Hashtable<Guid, ErrorCounter> failureCounts = new Hashtable<Guid, ErrorCounter>();
        private readonly IQueueStrategy queueStrategy;

        public ErrorAction(int numberOfRetries, IQueueStrategy queueStrategy)
        {
            this.numberOfRetries = numberOfRetries;
            this.queueStrategy = queueStrategy;
        }

        public void Init(IMsmqTrasport transport)
        {
            transport.MessageSerializationException += Transport_OnMessageSerializationException;
            transport.MessageProcessingFailure += Transport_OnMessageProcessingFailure;
            transport.MessageProcessingCompleted += Transport_OnMessageProcessingCompleted;
        }

        private void Transport_OnMessageSerializationException(CurrentMessageInformation information, Exception exception)
        {
            failureCounts.Write(writer => writer.Add(information.MessageId.Id, new ErrorCounter
            {
                FailureCount = numberOfRetries + 1,
                ExceptionText = exception.ToString()
            }));
        }

        private void Transport_OnMessageProcessingCompleted(CurrentMessageInformation information, Exception ex)
        {
            if (ex != null)
                return;

            ErrorCounter val = null;
            var id = information.MessageId.Id;
            failureCounts.Read(reader => reader.TryGetValue(id, out val));
            if (val == null)
                return;
            failureCounts.Write(writer => writer.Remove(id));
        }

        private void Transport_OnMessageProcessingFailure(CurrentMessageInformation information, Exception exception)
        {
            var id = information.MessageId.Id;
            failureCounts.Write(writer =>
            {
                ErrorCounter errorCounter;
                if (writer.TryGetValue(id, out errorCounter) == false)
                {
                    errorCounter = new ErrorCounter
                    {
                        ExceptionText = exception.ToString(),
                        FailureCount = 0
                    };
                    writer.Add(id, errorCounter);
                }
                errorCounter.FailureCount += 1;
            });
        }

        public bool CanHandlePeekedMessage(Message message)
        {
            return message.AppSpecific == 0;
        }

        public bool HandlePeekedMessage(MessageQueue queue, Message message)
        {
            var id = CorrelationId.Parse(message.Id).Id;
            ErrorCounter errorCounter = null;

            failureCounts.Read(reader => reader.TryGetValue(id, out errorCounter));

            if (errorCounter == null)
                return false;

            if (errorCounter.FailureCount < numberOfRetries)
                return false;

            failureCounts.Write(writer =>
            {
                writer.Remove(id);
                MoveToErrorQueue(queue, message, errorCounter.ExceptionText);
            });

            return true;
        }

        private void MoveToErrorQueue(MessageQueue queue, Message message, string exceptionText)
        {
            queueStrategy.MoveToErrorsQueue(queue, message);
            var label = "Error description for " + message.Label;
            if (label.Length > 249)
                label = label.Substring(0, 246) + "...";
            queue.Send(new Message
            {
                AppSpecific = (int)MessageType.ErrorDescriptionMessageMarker,
                Label = label,
                Body = exceptionText,
                CorrelationId = message.Id,
            }, queue.GetTransactionType());

            logger.WarnFormat("Moving message {0} to errors subqueue because: {1}", message.Id,
                              exceptionText);
        }

        private class ErrorCounter
        {
            public string ExceptionText;
            public int FailureCount;
        }

    }
}