namespace Rhino.ServiceBus.Messages
{
    public class AddSubscription : AdministrativeMessage
    {
        public string Type { get; set; }
        public string Endpoint { get; set; }
    }
}