using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace RabbitMQWorkshop;

public class Producer
{
    private const string QueueName = "token-events";
    private const string HostName = "localhost";
    private static string[] AvailableScopes = new[] { "nhn:sveleapi/svele/oppskrift", "nhn:sveleapi/svele/butteramount", "nhn:sveleapi/svele/ingredientlist" };

    public static async Task RunAsync()
    {
        var factory = new ConnectionFactory { HostName = HostName };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // Declare a queue (idempotent - safe to call multiple times)
        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        Console.WriteLine("Producer started. Press any key to send an event, or 'q' to quit.");

        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.KeyChar == 'q' || key.KeyChar == 'Q')
            {
                break;
            }

            var tokenEvent = new TokenIssuedEvent
            {
                ClientId = Guid.NewGuid().ToString(),
                TokenType = (TokenType)Random.Shared.Next(0, 3),
                ScopesRequested = AvailableScopes.Skip(Random.Shared.Next(0, 2)).Take(2).ToArray(),
                IssuedAt = DateTime.UtcNow
            };

            var message = JsonSerializer.Serialize(tokenEvent);
            var body = Encoding.UTF8.GetBytes(message);

            var props = new BasicProperties() { };
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: QueueName,
                body: body,
                mandatory: false,
                basicProperties: props
                );

            Console.WriteLine($"Sent [↑]: {tokenEvent.ClientId} {tokenEvent.TokenType.ToString()}");
        }

        Console.WriteLine("Producer stopped.");
    }
}
