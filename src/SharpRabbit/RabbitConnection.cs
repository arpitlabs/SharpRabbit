﻿using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;

namespace SharpRabbit
{
    internal class RabbitConnection : IRabbitConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly object _lock = new object();
        private readonly ILogger<RabbitConnection> _logger;
        private IConnection _connection;
        private bool _disposed;

        public RabbitConnection(ILogger<RabbitConnection> logger, IConnectionFactory connectionFactory)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _disposed = false;
        }

        public bool IsConnected => _connection?.IsOpen == true && !_disposed;

        public IModel CreateModel()
        {
            lock (_lock)
            {
                TryConnect();

                if (IsConnected)
                    return _connection.CreateModel();
                else
                    throw new BrokerUnreachableException(new Exception("No connection was stablished, therefore no Model was created"));
            }
        }

        public bool TryConnect()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (IsConnected)
                return true;

            var policy = Policy
                .Handle<BrokerUnreachableException>()
                .Or<SocketException>();

            try
            {
                policy
                    .Retry(3, (ex, retryNumber) => _logger.LogWarning(ex, $"There was an error while connecting to broker. Retry number: {retryNumber}"))
                    .Execute(() =>
                    {
                        _connection = _connectionFactory.CreateConnection();

                        _connection.ConnectionShutdown += OnConnectionShutdown;
                        _connection.ConnectionBlocked += OnConnectionBlocked;
                    });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "There was an error connecting to the specified broker, please check the error logs");
            }

            return IsConnected;
        }

        public void Dispose()
        {
            _logger.LogDebug("Initianting connection dispose");

            if (_disposed)
            {
                _logger.LogDebug("Connection already disposed");
                return;
            }

            try
            {
                _connection?.Close();
                _connection?.Dispose();
                _disposed = true;

                _logger.LogDebug("Connection successfully disposed");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        protected internal void OnConnectionBlocked(object sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            _logger.LogError($"The connection was blocked for the following reason: {e.Reason}. Trying to reconnect");

            TryConnect();
        }

        protected internal void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            switch (e.Initiator)
            {
                case ShutdownInitiator.Application:
                    {
                        _logger.LogInformation("Application requested connection shutdown");

                        break;
                    }
                case ShutdownInitiator.Peer:
                    {
                        _logger.LogError("The connection was shutdown by the Broker. Trying to reconnect");

                        TryConnect();
                        break;
                    }
            }
        }
    }
}
