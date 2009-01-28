using System;
using System.Messaging;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.LoadBalancer;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Msmq;
using Xunit;
using System.Linq;

namespace Rhino.ServiceBus.Tests.LoadBalancer
{
    public class With_load_balancing : LoadBalancingTestBase
    {
        private readonly IWindsorContainer container;

        public With_load_balancing()
        {
            var interpreter = new XmlInterpreter(@"LoadBalancer\BusWithLoadBalancer.config");
            container = new WindsorContainer(interpreter);
            container.Kernel.AddFacility("rhino.esb", new RhinoServiceBusFacility());

            container.AddComponent<MyHandler>();

            container.Register(
                Component.For<MsmqLoadBalancer>()
                    .DependsOn(new
                    {
                        threadCount = 1,
                        endpoint = new Uri(loadBalancerQueue)
                    })
                );
        }

        [Fact]
        public void Can_send_message_through_load_balancer()
        {
            MyHandler.ResetEvent = new ManualResetEvent(false);

            using (var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                loadBalancer.Start();
                bus.Start();

                bus.Send(loadBalancer.Endpoint, "abcdefg");

                MyHandler.ResetEvent.WaitOne();
                Assert.True(
                    MyHandler.Message.ResponseQueue.Path.Contains(@"private$\test_queue")
                    );

                Assert.Equal("abcdefg", MyHandler.Value);
            }
        }

        [Fact]
        public void When_worker_tell_load_balancer_that_it_is_ready_the_worker_will_be_added_to_known_queues()
        {
            using (var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                loadBalancer.Start();
                bus.Start();

                using (var workers = new MessageQueue(loadBalancerQueuePath + ";Workers", QueueAccessMode.SendAndReceive))
                {
                    workers.Formatter = new XmlMessageFormatter(new[] {typeof (string)});
                    var knownWorker = workers.Peek();
                    Assert.Equal(bus.Endpoint.Uri.ToString(), knownWorker.Body.ToString());
                }

                Assert.True(loadBalancer.KnownWorkers.GetValues().Contains(TestQueueUri.Uri));
            }
        }

        [Fact]
        public void When_load_balancer_starts_will_read_known_workers_from_workers_sub_queue()
        {
            using (var workers = new MessageQueue(loadBalancerQueuePath, QueueAccessMode.SendAndReceive))
            {
                workers.Formatter = new XmlMessageFormatter(new[] { typeof(string) });
                workers.Send(new Message(TestQueueUri.Uri.ToString()), workers.GetTransactionType());
                var peek = workers.Peek();
                string ignored;
                new SubQueueStrategy().TryMoveMessage(workers, peek, SubQueue.Workers, out ignored);
            }

            using (var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            {
                loadBalancer.Start();

                Assert.True(loadBalancer.KnownWorkers.GetValues().Contains(TestQueueUri.Uri));
            }
        }

        [Fact]
        public void When_new_end_point_send_message_to_load_balancer_the_end_point_will_be_persisted()
        {
            using (var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                loadBalancer.Start();
                bus.Start();

                bus.Send(bus.Endpoint, "test value");

                using (var endpoints = new MessageQueue(loadBalancerQueuePath + ";EndPoints", QueueAccessMode.SendAndReceive))
                {
                    endpoints.Formatter = new XmlMessageFormatter(new[] { typeof(string) });
                    var knownEndpoint = endpoints.Peek();
                    var busUri = bus.Endpoint.Uri.ToString().Replace("localhost", Environment.MachineName).ToLowerInvariant();
                    Assert.Equal(
                        busUri,
                        knownEndpoint.Body.ToString());
                    Assert.True(loadBalancer.KnownEndpoints.GetValues().Contains(new Uri(busUri)));
                }
            }
        }

        [Fact]
        public void When_load_balancer_starts_will_read_known_endpoints_from_endpoints_sub_queue()
        {
            using (var endPointsQueue = new MessageQueue(loadBalancerQueuePath, QueueAccessMode.SendAndReceive))
            {
                endPointsQueue.Formatter = new XmlMessageFormatter(new[] { typeof(string) });
                endPointsQueue.Send(new Message(TestQueueUri.Uri.ToString()), endPointsQueue.GetTransactionType());
                var peek = endPointsQueue.Peek();
                string ignored;
                new SubQueueStrategy().TryMoveMessage(endPointsQueue, peek, SubQueue.Endpoints, out ignored);
            }

            using (var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            {
                loadBalancer.Start();

                Assert.True(loadBalancer.KnownEndpoints.GetValues().Contains(TestQueueUri.Uri));
            }
        }

        [Fact]
        public void Will_send_administrative_messages_to_all_nodes()
        {
            using (var loadBalancer = container.Resolve<MsmqLoadBalancer>())
            using (var bus = container.Resolve<IStartableServiceBus>())
            {
                var wait = new ManualResetEvent(false);

                loadBalancer.MessageBatchSentToAllWorkers += message => wait.Set();

                loadBalancer.Start();
                bus.Start();

                bus.Send(loadBalancer.Endpoint, new ReadyToWork
                {
                    Endpoint = TransactionalTestQueueUri.Uri
                });

                bus.Send(loadBalancer.Endpoint, new ReadyToWork
                {
                    Endpoint = TestQueueUri2.Uri
                });

                bus.Send(loadBalancer.Endpoint, new AddSubscription
                {
                    Endpoint = bus.Endpoint.Uri.ToString(),
                    Type = "foobar"
                });

                wait.WaitOne();
            }

            using(var q = new MessageQueue(MsmqUtil.GetQueuePath(TransactionalTestQueueUri)))
            {
                var message = q.Receive(MessageQueueTransactionType.Single);
                Assert.Equal("Rhino.ServiceBus.Messages.AddSubscription", message.Label);
            }

            using (var q = new MessageQueue(MsmqUtil.GetQueuePath(TestQueueUri2)))
            {
                var message = q.Receive(MessageQueueTransactionType.Single);
                Assert.Equal("Rhino.ServiceBus.Messages.AddSubscription", message.Label);
            }
        }

        public class MyHandler : ConsumerOf<string>
        {
            public static ManualResetEvent ResetEvent;
            public static string Value;
            public static Message Message;

            public void Consume(string message)
            {
                Message = MsmqTransport.CurrentMessageInformation.MsmqMessage;
                Value = message;
                ResetEvent.Set();
            }

        }
    }
}
