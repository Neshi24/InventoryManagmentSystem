using EasyNetQ;
using InventoryService.Services;
using Shared;

namespace InventoryService.RabbitMQ
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
                
                var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();
               
                var connectionStr = "amqp://guest:guest@rabbitmq:5672/";
                
                var messageClient = new MessageClient(RabbitHutch.CreateBus(connectionStr));
                
                messageClient.Listen<MessageIdsDto>(
                    OnMessageReceived,
                    "orderCreation"
                );

                void OnMessageReceived(MessageIdsDto messageIds)
                {
                    try
                    {
                        itemService.GetMissingIds(messageIds);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine("MessageHandler is listening for order creation.");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}