using DistributedAsync.Abstractions;
using DistributedAsync.Tools;
using System.Threading.Tasks;

namespace DistributedAsync.Redis
{
    class RedisChannelFactory : IChannelFactory
    {
        private readonly RedisOptions _redisOptions;
        private readonly ISerializer _serializer;
        public RedisChannelFactory(RedisOptions redisOptions, ISerializer serializer = null)
        {
            _redisOptions = redisOptions;
            _serializer = serializer ?? new NewtonsoftSerializer();
        }

        public IChannelReader<TData> CreateChannelReader<TData>(string channelName) where TData : new()
        {
            var connection = RedisConnectionFactory.GetConnection(_redisOptions.ConnectionString);
            return _redisOptions.IsPersistence ?
                PersistenceRedisChannelReader<TData>.Subscribe(connection, _serializer, channelName, _redisOptions.PersistenceOption?.DataBaseNumber) :
                RedisChannelReader<TData>.Subscribe(connection, _serializer, channelName);
        }

        public async Task<IChannelReader<TData>> CreateChannelReaderAsync<TData>(string channelName) where TData : new()
        {
            var connection = RedisConnectionFactory.GetConnection(_redisOptions.ConnectionString);
            return _redisOptions.IsPersistence ?
                await PersistenceRedisChannelReader<TData>.SubscribeAsync(connection, _serializer, channelName, _redisOptions.PersistenceOption?.DataBaseNumber) :
                await RedisChannelReader<TData>.SubscribeAsync(connection, _serializer, channelName);
        }

        public IChannelWriter<TData> CreateChannelWriter<TData>(string channelName) where TData : new()
        {
            var connection = RedisConnectionFactory.GetConnection(_redisOptions.ConnectionString);
            return _redisOptions.IsPersistence ?
                new PersistenceRedisChannelWriter<TData>(connection, channelName, _serializer, _redisOptions.PersistenceOption?.DataBaseNumber, _redisOptions.PersistenceOption?.PersistenceExpiration):
                new RedisChannelWriter<TData>(connection, channelName, _serializer);
        }

    }
}
