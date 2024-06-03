using EasyNetQ;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;

namespace OrderService.RabbitMQ
{
    public class MessageClient : IDisposable
    {
        private readonly IBus _bus;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private readonly string _exchangeName;

        public MessageClient(IBus bus, string hostname, string exchangeName)
        {
            _bus = bus;
            
            var factory = new ConnectionFactory() { HostName = hostname };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchangeName = exchangeName;

            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);
            DeclareQueue("missingItems");
        }

        public void Consume<T>(Action<T> handler, string listenQueueName)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var messageObject = JsonConvert.DeserializeObject<T>(message);
                handler(messageObject);
            };

            _channel.BasicConsume(queue: listenQueueName,
                                autoAck: true, // Set to true if you want messages to be automatically acknowledged
                                consumer: consumer);
        }

        public void Publish<T>(T message, string publishQueueName)
        {
            Console.WriteLine("Publishing message");

            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            
            _channel.BasicPublish(exchange: _exchangeName,
                routingKey: publishQueueName,
                basicProperties: null,
                body: messageBody);

            Console.WriteLine("Published to queue: " + publishQueueName + " on exchange: " + _exchangeName);
            Console.WriteLine("Message published");
        }

        public void DeclareQueue(string listenQueueName)
        {
            _channel.QueueDeclare(queue: listenQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

             _channel.QueueBind(queue: listenQueueName,
                       exchange: _exchangeName,
                       routingKey: listenQueueName);
            
            Console.WriteLine("Queue declared: " + listenQueueName);
            Console.WriteLine("Queue bound to exchange: " + _exchangeName);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _bus?.Dispose();
        }
    }
}