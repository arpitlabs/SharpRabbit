# SharpRabbit

SharpRabbit is a simple and resilient API wrapper for the .NET Core RabbitMQ Client. 

The implementation code was initially forked from Microsoft's [eShopOnContainers example](https://github.com/dotnet-architecture/eShopOnContainers/blob/master/src/BuildingBlocks/EventBus/EventBusRabbitMQ/DefaultRabbitMQPersistentConnection.cs) and ported to a small classlib with some unit tests.

## Motivations

### Beware of no man's land (aka network) 💨

RabbitMQ.Client is a raw implementation of communication with a RabbitMQ broker via the *AMQP protocol* for .NET Core. Since this communication occurs in a not-so-controlled terrain called network, there might exist some issues while connecting to the broker.

SharpRabbit implements some simple *resiliency* patterns to ensure a proper and *health connection* from the application with the broker.

### No new frameworks, just the same API you love ❤

A lot of libraries that encapsulate RabbitMQ.Client like [Rebus](https://github.com/rebus-org/Rebus.RabbitMq) or [EasyNetQ](https://github.com/EasyNetQ/EasyNetQ)) are great and have their proper usage, but for some cases they add too much on top of the client itself. The goal here to provide *simplicity* and *flexibility with safety* around the native RabbitMQ.Client lib.

That said, SharpRabbit *does not aims to be an abstracted service bus*, but a *foundation* for it instead, so one can build its own the way it suits better.

### Good (re)usage of resources 🚀

Regarding use of connections, RabbitMQ.Client docs encourages [*long-living connections*](https://www.rabbitmq.com/connections.html) for the lifetime of the application. SharpRabbit aims to achieve that through DI and to do a gracious exit/cleanup when needed.

## Example with ASP.NET Core native DI

- In Startup.cs, register SharpRabbit to the ServiceCollection:

```csharp
services.AddSharpRabbit(Configuration.GetConnectionString("RabbitMQ"));
```

- In you consumer, publisher, bus, whatever:

```csharp
public class Whatever
{
  private readonly IRabbitConnection _connection;
  
  public Whatever(IRabbitConnection connection)
  {
    _connection = connection;
  }
  
  DoSomething()
  {
    if(_connection.TryConnect())
    {
      using(IModel model = _connection.CreateModel())
      {
        ...
      }
    }
  }
}
```

## Critics, suggestions and contributions

Are more than welcome! :)
