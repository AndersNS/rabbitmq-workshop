using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQWorkshop;

public class Consumer
{
    private const string QueueName = "token-events";
    private const string HostName = "localhost";

    public static async Task Run()
    {
        var factory = new ConnectionFactory { HostName = HostName };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // Declare the queue (idempotent - ensures queue exists)
        await channel.QueueDeclareAsync(
            queue: QueueName,
            // Queue stays even if RabbitMQ is restarted
            // https://www.rabbitmq.com/tutorials/tutorial-two-dotnet#message-durability
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // Set prefetch count to 1 for fair dispatch
        // https://www.rabbitmq.com/tutorials/tutorial-two-dotnet#fair-dispatch
        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

        Console.WriteLine($"Consumer waiting for messages. Press Ctrl+C to exit.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var tokenEvent = JsonSerializer.Deserialize<TokenIssuedEvent>(message);

                if (tokenEvent != null)
                {
                    Console.WriteLine($"Received [↓]:");
                    Console.WriteLine($"\t ClientId: {tokenEvent.ClientId}");
                    Console.WriteLine($"\t Token: {tokenEvent.TokenType.ToString()}");
                    Console.WriteLine($"\t Scopes: {string.Join(",", tokenEvent.ScopesRequested)}");
                    Console.WriteLine($"\t Created: {tokenEvent.IssuedAt:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine();

                    // Simulate processing
                    Thread.Sleep(1000);

                    // Acknowledge the message
                    await channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[!] Error processing message: {ex.Message}");
                // Reject and requeue the message
                await channel.BasicNackAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false, // Manual acknowledgment
            consumer: consumer);

        Console.WriteLine("Press any key to stop the consumer...");
        Console.ReadKey();
    }
}
