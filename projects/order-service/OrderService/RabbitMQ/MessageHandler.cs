using EasyNetQ;
using Shared;

namespace OrderService.RabbitMQ;

public class MessageHandler : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var connectionStr = "amqp://guest:guest@rabbitmq:5672/";
                
            var messageClient = new MessageClient(RabbitHutch.CreateBus(connectionStr));
            
            messageClient.Listen<MessageIds>(
                OnMessageReceived,
                "missingItems"
            );

            void OnMessageReceived(MessageIds messageIds)
            {
                try
                {
                    Console.WriteLine($"Received message: OrderId = {messageIds.OrderId}, ItemsIds = [{string.Join(", ", messageIds.ItemsIds)}]");
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
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("MessageHandler is stopping.");
        }
    }
}