# RabbitMQ Workshop - TokenIssuedEvents

A simple C# starter project demonstrating RabbitMQ message publishing and consuming using the RabbitMQ.Client library.

- **Producer**: Sends messages to a RabbitMQ queue
- **Consumer**: Receives and processes messages from the queue

## Running the Workshop

### 1. Start RabbitMQ

```bash
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management
```

Access at management studio at: [http://localhost:15672](http://localhost:15672)

(Username `guest`, Password `guest`)

> [!TIP]
> You may have to open an incognito/private browser window to access the management UI. Because of too many headers being set from previous localhost sessions.

### 2. Build the Project

```bash
cd RabbitMQWorkshop
dotnet restore
dotnet build
```

### 3. Run the Producer

Open a terminal and run:

```bash
dotnet run
```

Choose option `1` for Producer mode. Press any key to send messages, press `q` to quit.

### 4. Run the Consumer

Open a **second terminal** and run:

```bash
dotnet run
```

Choose option `2` for Consumer mode. It will start receiving messages.

### 5. Excrcises!

Try these scenarios:

1. Start multiple consumers (run `dotnet run` and select option 2 in multiple terminals)
2. Add a new field to the event
3. Stop the consumer and send messages from producer
   a. Start the consumer again - do messages come through?
   b. What if you restart RabbitMQ while the consumer is stopped?
4. Check the RabbitMQ Management UI to see queue statistics
5. Create a consumer that only processes messages for specific tokentypes (Hint: Look at routing)
6. Create a consumer that processes messages for specific tokentypes AND specific scopes (Hint: look at topics)
7. Error handling. Make the consumer randomly fail and observe message requeuing.
   a. Bonus createa a dead letter queue for failed messages

## Resources

- [RabbitMQ Tutorials](https://www.rabbitmq.com/tutorials)
- [RabbitMQ .NET Client Guide](https://www.rabbitmq.com/dotnet-api-guide.html)
