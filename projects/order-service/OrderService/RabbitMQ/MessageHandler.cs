using EasyNetQ;
using Shared;

namespace OrderService.RabbitMQ
{
    public class MessageHandler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                
                var connectionStr = "amqp://guest:guest@rabbitmq:5672/";

              
                var messageClient = new MessageClient(RabbitHutch.CreateBus(connectionStr));
                
                messageClient.Listen<List<Item>>(
                    OnMessageReceived,
                    "notificationSubscription"
                );

                void OnMessageReceived(List<Item> item)
                {
                    try
                    {
                        //TODO add items to order
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine("MessageHandler is listening for notifications.");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}