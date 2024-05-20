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
            var bus = RabbitHutch.CreateBus(connectionStr);

            var messageClient = new MessageClient(bus);
            
            messageClient.Listen<MessageIds>(message =>
            {
                Console.WriteLine($"Received message: {message}");
            }, "missingItems");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("MessageHandler is listening.");
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