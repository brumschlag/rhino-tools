using System;
using System.Messaging;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.Msmq;
using Rhino.ServiceBus.Serializers;

namespace Rhino.ServiceBus.Tests
{
    public class MsmqFlatQueueTestBase : IDisposable
    {
        private readonly string subbscriptionQueuePath;
        protected readonly Uri SubscriptionsUri;

        protected readonly string testQueuePath;
        protected readonly Uri TestQueueUri;

        protected readonly string testQueuePath2;
        protected readonly Uri TestQueueUri2;

        protected readonly string transactionalTestQueuePath;
        protected readonly Uri TransactionalTestQueueUri;

        protected MessageQueue queue;
        protected MessageQueue subscriptions;
        protected MessageQueue transactionalQueue;

        private ITransport transactionalTransport;
        private ITransport transport;
        protected readonly MessageQueue testQueue2;

        public MsmqFlatQueueTestBase()
        {
            TestQueueUri = new Uri("msmq://localhost/test_queue");
            testQueuePath = MsmqUtil.GetQueuePath(TestQueueUri);

            TestQueueUri2 = new Uri("msmq://localhost/test_queue2");
            testQueuePath2 = MsmqUtil.GetQueuePath(TestQueueUri2);

            TransactionalTestQueueUri = new Uri("msmq://localhost/transactional_test_queue");
            transactionalTestQueuePath = MsmqUtil.GetQueuePath(TransactionalTestQueueUri);

            SubscriptionsUri = TestQueueUri;
            subbscriptionQueuePath = MsmqUtil.GetQueuePath(SubscriptionsUri);

            var errorsPathSuffix= "#errors";
            var discardedPathSuffix= "#discarded";

            if (MessageQueue.Exists(testQueuePath) == false)
                MessageQueue.Create(testQueuePath);

            if (MessageQueue.Exists(testQueuePath2) == false)
                MessageQueue.Create(testQueuePath2);

            if (MessageQueue.Exists(transactionalTestQueuePath) == false)
                MessageQueue.Create(transactionalTestQueuePath, true);

            if (MessageQueue.Exists(testQueuePath+errorsPathSuffix) == false)
                MessageQueue.Create(testQueuePath + errorsPathSuffix);

            if (MessageQueue.Exists(testQueuePath2 + errorsPathSuffix) == false)
                MessageQueue.Create(testQueuePath2 + errorsPathSuffix);

            if (MessageQueue.Exists(transactionalTestQueuePath + errorsPathSuffix) == false)
                MessageQueue.Create(transactionalTestQueuePath + errorsPathSuffix,true);

            if (MessageQueue.Exists(testQueuePath + discardedPathSuffix) == false)
                MessageQueue.Create(testQueuePath + discardedPathSuffix);

            queue = new MessageQueue(testQueuePath);
            queue.Purge();

            using (var errQueue = new MessageQueue(testQueuePath + errorsPathSuffix))
            {
                errQueue.Purge();
            }
            using (var discardedQueue = new MessageQueue(testQueuePath + discardedPathSuffix))
            {
                discardedQueue.Purge();
            }

            testQueue2 = new MessageQueue(testQueuePath2);
            testQueue2.Purge();

            using (var errQueue2 = new MessageQueue(testQueuePath2 + errorsPathSuffix))
            {
                errQueue2.Purge();
            }

            transactionalQueue = new MessageQueue(transactionalTestQueuePath);
            transactionalQueue.Purge();

            using (var errQueue3 = new MessageQueue(transactionalTestQueuePath + errorsPathSuffix))
            {
                errQueue3.Purge();
            }

            subscriptions = new MessageQueue(subbscriptionQueuePath)
            {
                Formatter = new XmlMessageFormatter(new[] { typeof(string) })
            };
            subscriptions.Purge();
        }

        public ITransport Transport
        {
            get
            {
                if (transport == null)
                {
                    transport = new MsmqTransport(new XmlMessageSerializer(new DefaultReflection()), TestQueueUri, 1, 5,
                        new FlatQueueStrategy(TestQueueUri));
                    transport.Start();
                }
                return transport;
            }
        }

        public ITransport TransactionalTransport
        {
            get
            {
                if (transactionalTransport == null)
                {
                    transactionalTransport = new MsmqTransport(new XmlMessageSerializer(new DefaultReflection()),
                                                               TransactionalTestQueueUri, 1, 5,new FlatQueueStrategy(TransactionalTestQueueUri));
                    transactionalTransport.Start();
                }
                return transactionalTransport;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            queue.Dispose();
            transactionalQueue.Dispose();
            subscriptions.Dispose();

            if (transport != null)
                transport.Stop();
            if (transactionalTransport != null)
                transactionalTransport.Stop();
        }

        #endregion
    }
}
