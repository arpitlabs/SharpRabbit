using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Xunit;
using Xunit.Abstractions;

namespace SharpRabbit.Tests.RabbitConnection
{
    public class DisposeTestes
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ILogger<SharpRabbit.RabbitConnection> _logger;

        public DisposeTestes(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _logger = _testOutputHelper.BuildLoggerFor<SharpRabbit.RabbitConnection>();
        }

        [Fact]
        public void DisposeCalled_CallsConnectionClose()
        {
            var connectionFactoryMock = new Mock<IConnectionFactory>();
            var connectionMock = new Mock<IConnection>();

            connectionFactoryMock
                .Setup(c => c.CreateConnection())
                .Returns(connectionMock.Object);

            var rabbitConnection = new SharpRabbit.RabbitConnection(
                    _logger,
                    connectionFactoryMock.Object);

            rabbitConnection.TryConnect();

            rabbitConnection.Dispose();

            connectionMock.Verify(c => c.Close(), Times.Once);
        }

        [Fact]
        public void DisposeCalled_CallsConnectionDispose()
        {
            var connectionFactoryMock = new Mock<IConnectionFactory>();
            var connectionMock = new Mock<IConnection>();

            connectionFactoryMock
                .Setup(c => c.CreateConnection())
                .Returns(connectionMock.Object);

            var rabbitConnection = new SharpRabbit.RabbitConnection(
                    _logger,
                    connectionFactoryMock.Object);

            rabbitConnection.TryConnect();

            rabbitConnection.Dispose();

            connectionMock.Verify(c => c.Dispose(), Times.Once);
        }
    }
}
