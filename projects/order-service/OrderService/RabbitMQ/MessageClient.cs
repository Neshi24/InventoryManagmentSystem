using EasyNetQ;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace OrderService.RabbitMQ
{
    public class MessageClient : IDisposable
    {
        private readonly IBus _bus;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageClient(IBus bus, string hostname)
        {
            _bus = bus;
            
            var factory = new ConnectionFactory() { HostName = hostname };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void Listen<T>(Action<T> handler, string subscriptionId)
        {
            _bus.PubSub.Subscribe<T>(subscriptionId, handler);
        }

        public void Publish<T>(T message, string queueName)
        {
            Console.WriteLine("Publishing message");

            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            
            _channel.BasicPublish(exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: messageBody);

            Console.WriteLine("Message published");
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _bus?.Dispose();
        }
    }
}