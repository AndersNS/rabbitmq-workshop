using RabbitMQWorkshop;

Console.WriteLine("RabbitMQ Workshop - Order Processing Demo");
Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Choose mode:");
Console.WriteLine("1. Producer - Send order events");
Console.WriteLine("2. Consumer - Receive order events");
Console.WriteLine();
Console.Write("Enter your choice (1 or 2): ");

var choice = Console.ReadLine();

Console.WriteLine();

try
{
    switch (choice)
    {
        case "1":
            await Producer.RunAsync();
            break;
        case "2":
            await Consumer.Run();
            break;
        default:
            Console.WriteLine("Invalid choice. Please run the program again and select 1 or 2.");
            break;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("Make sure RabbitMQ is running on localhost:5672");
    Console.WriteLine("You can start RabbitMQ using the command:");
    Console.WriteLine("\t docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management");
}
