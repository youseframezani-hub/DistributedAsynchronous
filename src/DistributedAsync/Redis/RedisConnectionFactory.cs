using StackExchange.Redis;
using System;
using System.Collections.Concurrent;

namespace DistributedAsync.Redis
{
    static class RedisConnectionFactory
    {
        public static ConnectionMultiplexer GetConnection(string connectionString)
        {
            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (ConnectionStringToInstance.TryGetValue(connectionString, out var connection) && connection.IsConnected)
            {
                return connection;
            }

            RemoveConnection(connectionString);

            lock (KeyLock)
            {
                return AddConnection(connectionString);
            }

        }

        private static readonly object KeyLock = new object();
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionStringToInstance = new ConcurrentDictionary<string, ConnectionMultiplexer>();

        private static ConnectionMultiplexer AddConnection(string connectionString)
        {
            try
            {
                if (ConnectionStringToInstance.TryGetValue(connectionString, out var connection) && connection.IsConnected)
                {
                    return connection;
                }

                ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
                options.AbortOnConnectFail = false;
                options.AsyncTimeout = 10000;
                options.SyncTimeout = 10000;
                options.ConnectRetry = 5;

                connection = ConnectionMultiplexer.Connect(options);

                if (connection.IsConnected)
                {
                    ConnectionStringToInstance.AddOrUpdate(connectionString, connection, (k, v) => connection);
                    return connection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"failed to connect {connectionString}", ex);
            }
            return null;
        }
        private static void RemoveConnection(string connectionString)
        {
            ConnectionStringToInstance.TryRemove(connectionString, out var connection);
            if (connection == null)
            {
                return;
            }

            CloseConnection(connection);
        }

        private static void CloseConnection(ConnectionMultiplexer oldConnection)
        {
            if (oldConnection == null)
                return;

            try
            {
                oldConnection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"failed to close redis connection {oldConnection.Configuration}", ex);
            }
        }
    }
}
