
using OrderService.Services;
using Shared;


namespace OrderService.RabbitMQ
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
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                _messageClient.Listen<MessageIdsDto>(
                    OnMessageReceived,
                    "missingItems"
                );

                void OnMessageReceived(MessageIdsDto messageIds)
                {
                    try
                    {
                        Console.WriteLine(
                            $"Received message: OrderId = {messageIds.OrderId}, ItemsIds = [{string.Join(", ", messageIds.ItemsIds)}]");
                        orderService.CreateMissingItemHistory(messageIds);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine("MessageHandler is listening for missing items.");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}