namespace Starbucks.Messages
{
    public class NewOrder
    {
        public string DrinkName { get; set; }

        public DrinkSize Size { get; set; }
        
        public string CustomerName { get; set; }
    }
}