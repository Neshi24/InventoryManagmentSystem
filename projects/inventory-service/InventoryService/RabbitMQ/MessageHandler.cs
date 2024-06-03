using InventoryService.Services;
using Shared;

namespace InventoryService.RabbitMQ
{
    public class MessageHandler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageClient _messageClient;

        public MessageHandler(IServiceProvider serviceProvider, MessageClient messageClient)
        {
            _serviceProvider = serviceProvider;
            _messageClient = messageClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();

                

                _messageClient.Consume<MessageIdsDto>(
                    OnMessageReceived,
                    "orderCreation"
                );

                void OnMessageReceived(MessageIdsDto messageIds)
                {
                    try
                    {
                        itemService.GetMissingIds(messageIds);
                        Console.WriteLine("I got something");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine("MessageHandler is listening for order creation.");
                    await Task.Delay(10000, stoppingToken);
                }
            }
        }
    }
}