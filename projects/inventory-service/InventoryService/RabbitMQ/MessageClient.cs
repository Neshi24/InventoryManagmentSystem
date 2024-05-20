using EasyNetQ;

namespace InventoryService.RabbitMQ
{
    public class MessageClient
    {
        private readonly IBus _bus;

        public MessageClient(IBus bus)
        {
            _bus = bus;
        }

        public void Listen<T>(Action<T> handler, string subscriptionId)
        {
            _bus.PubSub.Subscribe<T>(subscriptionId, handler);
        }
        
        
        public void Publish<T>(T message)
        {
            Console.WriteLine("Publishing message");
            _bus.PubSub.Publish(message);
            Console.WriteLine("Message published");
        }
    }
}