using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Rhino.ServiceBus.Impl;
using Rhino.ServiceBus.Serializers;
using Xunit;

namespace Rhino.ServiceBus.Tests
{
    public class XmlSerializerTest
    {
        private readonly Order sample = new Order
        {
            Url = new Uri("msmq://www.ayende.com/"),
            At = DateTime.Today,
            Count = 5,
            OrderId = new Guid("1909994f-8173-452c-a651-14725bb09cb6"),
            OrderLines = new[]
            {
                new OrderLine
                {
                    Product = "milk",
                    Fubar = new List<int>{1,2,3}
                },
                new OrderLine
                {
                    Product = "butter",
                    Fubar = new List<int>{4,5,6}
                }
            },
            TimeToDelivery = TimeSpan.FromDays(1),
        };

        [Fact]
        public void Can_serialize_complex_object_graph()
        {
            var serializer = new XmlMessageSerializer(new DefaultReflection());
            var stream = new MemoryStream();
            serializer.Serialize(new[] { sample }, stream);
            stream.Position = 0;
            var serializedForm = new StreamReader(stream,Encoding.UTF8).ReadToEnd();
            var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<esb:messages xmlns:esb=""http://servicebus.hibernatingrhinos.com/2008/12/20/esb"" xmlns:list=""http://servicebus.hibernatingrhinos.com/2008/12/20/list"" xmlns:tests.order=""Rhino.ServiceBus.Tests.XmlSerializerTest+Order, Rhino.ServiceBus.Tests"" xmlns:tests.orderline=""Rhino.ServiceBus.Tests.XmlSerializerTest+OrderLine, Rhino.ServiceBus.Tests"">
  <tests.order:Order>
    <Url>msmq://www.ayende.com/</Url>
    <Count>5</Count>
    <OrderId>1909994f-8173-452c-a651-14725bb09cb6</OrderId>
    <At>2008-12-15T00:00:00.0000000</At>
    <TimeToDelivery>P0Y0M1DT0H0M0S</TimeToDelivery>
    <list:OrderLines>
      <tests.orderline:value>
        <Product>milk</Product>
        <list:Fubar>
          <value>1</value>
          <value>2</value>
          <value>3</value>
        </list:Fubar>
      </tests.orderline:value>
      <tests.orderline:value>
        <Product>butter</Product>
        <list:Fubar>
          <value>4</value>
          <value>5</value>
          <value>6</value>
        </list:Fubar>
      </tests.orderline:value>
    </list:OrderLines>
  </tests.order:Order>
</esb:messages>";
            Assert.Equal(expected,serializedForm);
        }

        [Fact]
        public void Can_serialize_and_deserialize_primitive()
        {
            var ticks = DateTime.Now.Ticks;
            var serializer = new XmlMessageSerializer(new DefaultReflection());
            var stream = new MemoryStream();
            serializer.Serialize(new object[]{ticks},stream);
            stream.Position = 0;
            var actual = (long)serializer.Deserialize(stream)[0];
            Assert.Equal(ticks, actual);
        }

        [Fact]
        public void Can_deserialize_complex_object_graph()
        {
            var serializer = new XmlMessageSerializer(new DefaultReflection());
            var stream = new MemoryStream();
            serializer.Serialize(new[] { sample }, stream);
            stream.Position = 0;
            var order = (Order)serializer.Deserialize(stream)[0];

            Assert.Equal(sample.Url, order.Url);
            Assert.Equal(sample.At, order.At);
            Assert.Equal(sample.Count, order.Count);
            Assert.Equal(sample.OrderId, order.OrderId);
            Assert.Equal(sample.TimeToDelivery, order.TimeToDelivery);

            Assert.Equal(2, order.OrderLines.Length);

            Assert.Equal(sample.OrderLines[0].Product, order.OrderLines[0].Product);
            Assert.Equal(sample.OrderLines[1].Product, order.OrderLines[1].Product);
        }

        #region Nested type: Order

        public class Order
        {
            public Uri Url { get; set; }
            public int Count { get; set; }
            public Guid OrderId { get; set; }
            public DateTime At { get; set; }
            public TimeSpan TimeToDelivery { get; set; }

            public OrderLine[] OrderLines { get; set; }
        }

        #endregion

        #region Nested type: OrderLine

        public class OrderLine
        {
            public string Product { get; set; }
            public List<int> Fubar { get; set; }
        }

        #endregion
    }
}