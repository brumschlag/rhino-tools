using System;
using Rhino.ServiceBus.Sagas;

namespace Rhino.ServiceBus.Internal
{
    public interface IReflection
    {
        object CreateInstance(string typeName);

        void Set(object instance, string name, object value);
        
        object ForAllOf<T>(object instance, Func<T, T> func);
        
        Type GetGenericTypeOf(Type type, object msg);
        
        Type GetGenericTypeOf(Type type, Type paramType);
        
        void InvokeConsume(object consumer, object msg);
        
        Type[] GetMessagesConsumes(IMessageConsumer consumer);

        object InvokeSagaPersisterGet(object persister, Guid correlationId);

        void InvokeSagaPersisterSave(object persister, ISaga entity);
        void InvokeSagaPersisterComplete(object persister, ISaga entity);
    }
}