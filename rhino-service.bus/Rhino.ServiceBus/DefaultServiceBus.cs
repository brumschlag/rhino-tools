using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using log4net;
using Rhino.ServiceBus.Exceptions;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Internal;
using Rhino.ServiceBus.MessageModules;
using Rhino.ServiceBus.Messages;
using Rhino.ServiceBus.Sagas;
using Rhino.ServiceBus.Util;

namespace Rhino.ServiceBus
{
    public class DefaultServiceBus : IStartableServiceBus
    {
        private readonly IKernel kernel;

        private readonly ILog logger = LogManager.GetLogger(typeof (DefaultServiceBus));
        private readonly IMessageModule[] modules;
        private readonly IReflection reflection;
        private readonly ISubscriptionStorage subscriptionStorage;
        private readonly ITransport transport;

        public DefaultServiceBus(
            IKernel kernel,
            ITransport transport,
            ISubscriptionStorage subscriptionStorage,
            IReflection reflection,
            IMessageModule[] modules
            )
        {
            this.transport = transport;
            this.subscriptionStorage = subscriptionStorage;
            this.reflection = reflection;
            this.modules = modules;
            this.kernel = kernel;
        }

        public IMessageModule[] Modules
        {
            get { return modules; }
        }

        #region IStartableServiceBus Members

        public void Publish(params object[] messages)
        {
            if (PublishInternal(messages) == false)
                throw new MessagePublicationException("There were no subscribers for (" +
                                                      messages.First() + ")"
                    );
        }

        public void Notify(params object[] messages)
        {
            PublishInternal(messages);
        }

        public void Reply(params object[] messages)
        {
            transport.Reply(messages);
        }

        public void Send(Uri endpoint, params object[] messages)
        {
            transport.Send(endpoint, messages);
        }

        public IServiceBus AddInstanceSubscription(IMessageConsumer consumer)
        {
            subscriptionStorage.AddInstanceSubscription(consumer);
            return this;
        }

        public Uri Endpoint
        {
            get { return transport.Endpoint; }
        }

        public void Dispose()
        {
            transport.Stop();
            transport.MessageArrived -= Transport_OnMessageArrived;
            transport.ManagementMessageArrived -= Transport_OnManagementMessageArrived;

            foreach (IMessageModule module in modules)
            {
                module.Stop(transport);
            }
        }

        public void Start()
        {
            foreach (IMessageModule module in modules)
            {
                module.Init(transport);
            }
            transport.MessageArrived += Transport_OnMessageArrived;
            transport.ManagementMessageArrived += Transport_OnManagementMessageArrived;
            transport.Start();
        }

        #endregion

        private bool PublishInternal(object[] messages)
        {
            bool sentMsg = false;
            if (messages.Length == 0)
                throw new MessagePublicationException("Cannot publish an empty message batch");
            object msg = messages[0];
            IEnumerable<Uri> subscriptions = subscriptionStorage.GetSubscriptionsFor(msg.GetType());
            foreach (Uri subscription in subscriptions)
            {
                transport.Send(subscription, messages);
                sentMsg = true;
            }
            return sentMsg;
        }

        private void Transport_OnManagementMessageArrived(CurrentMessageInformation msg)
        {
            var addSubscription = msg.Message as AddSubscription;
            if (addSubscription != null)
            {
                subscriptionStorage.AddSubscription(addSubscription.Type, addSubscription.Endpoint);
                return;
            }
            var removeSubscription = msg.Message as RemoveSubscription;
            if (removeSubscription != null)
            {
                subscriptionStorage.RemoveSubscription(removeSubscription.Type, removeSubscription.Endpoint);
                return;
            }
            logger.WarnFormat("Got unknown management message for management endpoint: {0}", msg.Message);
        }

        public void Transport_OnMessageArrived(CurrentMessageInformation msg)
        {
            object[] consumers = GatherConsumers(msg);

            if (consumers.Length == 0)
            {
                logger.ErrorFormat("Got message {0}, but had no consumers for it", msg);
                return;
            }
            try
            {
                foreach (object consumer in consumers)
                {
                    reflection.InvokeConsume(consumer, msg.Message);

                    var sagaEntity = consumer as ISaga;
                    if (sagaEntity == null)
                        continue;
                    PersistSagaInstance(sagaEntity);
                }
            }
            finally
            {
                foreach (object consumer in consumers)
                {
                    kernel.ReleaseComponent(consumer);
                }
            }
        }

        private void PersistSagaInstance(ISaga saga)
        {
            Type persisterType = reflection.GetGenericTypeOf(typeof (ISagaPersister<>), saga);
            object persister = kernel.Resolve(persisterType);

            if (saga.IsCompleted)
                reflection.InvokeSagaPersisterComplete(persister, saga);
            else
                reflection.InvokeSagaPersisterSave(persister, saga);
        }

        private object[] GatherConsumers(CurrentMessageInformation msg)
        {
            object[] sagas = GetSagasFor(msg.Message as ISagaMessage);

            object[] instanceConsumers = subscriptionStorage
                .GetInstanceSubscriptions(msg.Message.GetType());

            Type consumerType = reflection.GetGenericTypeOf(typeof (ConsumerOf<>), msg.Message);
            var consumers = (object[]) kernel.ResolveAll(consumerType, new Hashtable());
            return instanceConsumers
                .Union(sagas)
                .Union(consumers)
                .ToArray();
        }

        private object[] GetSagasFor(ISagaMessage sagaMessage)
        {
            if (sagaMessage == null)
                return new object[0];

            var instances = new List<object>();
            Type sagaInitiatedByThisMessage = reflection.GetGenericTypeOf(typeof (InitiatedBy<>), sagaMessage);

            var initiated = (object[]) kernel.ResolveAll(sagaInitiatedByThisMessage, new Hashtable());

            foreach (ISaga saga in initiated)
            {
                saga.Id = GuidCombGenerator.Generate();
            }

            instances.AddRange(initiated);

            Type messageType = reflection.GetGenericTypeOf(typeof (Orchestrates<>), sagaMessage);

            IHandler[] handlers = kernel.GetAssignableHandlers(messageType);

            foreach (IHandler sagaPersisterHandler in handlers)
            {
                Type sagaPersisterType = reflection.GetGenericTypeOf(typeof (ISagaPersister<>),
                                                                     sagaPersisterHandler.ComponentModel.Implementation);

                object sagaPersister = kernel.Resolve(sagaPersisterType);
                try
                {
                    object sagaInstance = reflection.InvokeSagaPersisterGet(sagaPersister, sagaMessage.CorrelationId);
                    if (sagaInstance != null)
                        continue;
                    instances.Add(sagaInstance);
                }
                finally
                {
                    kernel.ReleaseComponent(sagaPersister);
                }
            }
            return instances.ToArray();
        }
    }
}