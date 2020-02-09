# SharpRabbit

SharpRabbit is a simple and resilient API wrapper for the .NET Core RabbitMQ Client.

## Motivations

### Beware of no man's land (aka network) 💨

RabbitMQ.Client is a raw implementation of communication with a RabbitMQ broker via the AMQP protocol for .NET Core. Since this communication occurs in a not-so-controlled terrain called network, there might exist some issues while connecting to the broker.

SharpRabbit implements some simple resiliency patterns to ensure a proper and health application connection with the broker.

### No new frameworks, just the same API you love ❤

A lot of libraries that encapsulate RabbitMQ.Client (Rebus, EasyNetQ) are great and have their proper usage, but for some cases they add too much on top of the client itself. SharpRabbit *does not aims to be an abstracted service bus*, but a *foundation* for it instead, so one can build its own the way it looks better. The goal is to provide *simplicity* and *flexibility with safety* around the native RabbitMQ.Client lib.

### Good (re)usage of resources 🚀

Regarding use of connections, RabbitMQ.Client docs encourages long-running connections for the lifetime of the application. SharpRabbit aims to achieve that and do a gracious exit/cleanup when needed.


## Critics, suggestions and contributions

Are more than welcome! :)
