
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

                _messageClient.Consume<MessageIdsDto>(
                    OnMessageReceived,
                    "missingItems"
                );

                async void OnMessageReceived(MessageIdsDto messageIds)
                {
                    try
                    {
                        await orderService.CreateMissingItemHistory(messageIds);

                        Console.WriteLine(
                            $"Received message: OrderId = {messageIds.OrderId}, ItemsIds = [{string.Join(", ", messageIds.ItemsIds)}]");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine("MessageHandler is listening for missing items.");
                    await Task.Delay(10000, stoppingToken);
                }
            }
        }
    }
}