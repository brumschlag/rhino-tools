using System;
using System.Transactions;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.Mocks;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.MessageModules;
using Rhino.ServiceBus.Messages;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class MessageLoggingTests : MsmqTestBase
    {
        private readonly IWindsorContainer container;
        private ITransport transport;
        private IMessageSerializer messageSerializer;

        public MessageLoggingTests()
        {
            container = new WindsorContainer(new XmlInterpreter());
            container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility());
            container.AddComponent<MessageLoggingModule>();

            messageSerializer = container.Resolve<IMessageSerializer>();

            transport = MockRepository.GenerateStub<ITransport>();
        }

        [Fact]
        public void Will_send_message_about_serialization_failure()
        {
            var module = container.Resolve<MessageLoggingModule>(new { logQueue = TestQueueUri });
            module.Init(transport);

            transport.Raise(x => x.MessageSerializationException += null,
                new CurrentMessageInformation { MessageId = CorrelationId.New() },
                new InvalidOperationException());

            var msg = queue.Receive();

            var serializationError = (SerializationErrorMessage)messageSerializer.Deserialize(msg.BodyStream)[0];
            Assert.Equal("System.InvalidOperationException: Operation is not valid due to the current state of the object.", serializationError.Error);
            Assert.NotEqual(CorrelationId.Empty, serializationError.MessageId);
        }

        [Fact]
        public void Will_send_message_about_message_arrived()
        {
            var module = container.Resolve<MessageLoggingModule>(new { logQueue = TestQueueUri });
            module.Init(transport);

            transport.Raise(x => x.MessageArrived += null,
                new CurrentMessageInformation
                {
                    MessageId = CorrelationId.New(),
                    Message = "tst"
                });

            var msg = queue.Receive();

            var messageArrivedMessage = (MessageArrivedMessage)messageSerializer.Deserialize(msg.BodyStream)[0];
            Assert.NotEqual(CorrelationId.Empty, messageArrivedMessage.MessageId);
            Assert.Equal("tst", messageArrivedMessage.Message);
        }

        [Fact]
        public void Will_send_message_about_message_processing_completed()
        {
            var module = container.Resolve<MessageLoggingModule>(new { logQueue = TestQueueUri });
            module.Init(transport);

            transport.Raise(x => x.MessageProcessingCompleted += null,
                            new CurrentMessageInformation
                            {
                                MessageId = CorrelationId.New(),
                                Message = "tst"
                            },
                            new Exception());

            var msg = queue.Receive();

            var processingCompletedMessage = (MessageProcessingCompletedMessage)messageSerializer.Deserialize(msg.BodyStream)[0];
            Assert.NotEqual(CorrelationId.Empty, processingCompletedMessage.MessageId);
        }

        [Fact]
        public void Will_send_message_about_message_processing_failed()
        {
            var module = container.Resolve<MessageLoggingModule>(new { logQueue = TestQueueUri });
            module.Init(transport);

            transport.Raise(x => x.MessageProcessingFailure += null,
                new CurrentMessageInformation
                {
                    MessageId = CorrelationId.New(),
                    Message = "tst"
                },
                new IndexOutOfRangeException());

            var msg = queue.Receive();

            var failedMessage = (MessageProcessingFailedMessage)messageSerializer.Deserialize(msg.BodyStream)[0];
            Assert.NotEqual(CorrelationId.Empty, failedMessage.MessageId);
            Assert.Equal("System.IndexOutOfRangeException: Index was outside the bounds of the array.", failedMessage.ErrorText);
            Assert.Equal("tst",failedMessage.Message);
        }

        [Fact]
        public void Will_send_message_about_message_sent()
        {
            var module = container.Resolve<MessageLoggingModule>(new { logQueue = TestQueueUri });
            module.Init(transport);

            transport.Raise(x => x.MessageSent += null,
                new CurrentMessageInformation
                {
                    MessageId = CorrelationId.New(),
                    AllMessages = new[]{"test"}
                });

            var msg = queue.Receive();

            var failedMessage = (MessageSentMessage)messageSerializer.Deserialize(msg.BodyStream)[0];
            Assert.NotEqual(CorrelationId.Empty, failedMessage.MessageId);
            Assert.Equal(new[] { "test" }, failedMessage.Message);
        }

        [Fact]
        public void Will_send_message_about_message_processing_failed_even_when_rolling_back_tx()
        {
            var module = container.Resolve<MessageLoggingModule>(new { logQueue = TestQueueUri });
            module.Init(transport);

            using(new TransactionScope())
            {
                transport.Raise(x => x.MessageProcessingFailure += null,
                new CurrentMessageInformation
                {
                    MessageId = CorrelationId.New(),
                    Message = "tst"
                },
                new IndexOutOfRangeException());
            }

            var msg = queue.Receive();

            var failedMessage = (MessageProcessingFailedMessage)messageSerializer.Deserialize(msg.BodyStream)[0];
            Assert.NotEqual(CorrelationId.Empty, failedMessage.MessageId);
            Assert.Equal("System.IndexOutOfRangeException: Index was outside the bounds of the array.", failedMessage.ErrorText);
            Assert.Equal("tst", failedMessage.Message);
        }
    }
}