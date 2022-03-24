using DistributedAsync.Abstractions;
using System.Threading.Tasks;

namespace DistributedAsync.Redis
{
    class RedisChannelFactory : IChannelFactory
    {
        private readonly string _connectionString;
        public RedisChannelFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IChannelReader<TData> CreateChannelReader<TData>(string channelName) where TData : new()
        {
            return RedisChannelReader<TData>.Subscribe(RedisConnectionFactory.GetConnection(_connectionString), channelName);
        }

        public async Task<IChannelReader<TData>> CreateChannelReaderAsync<TData>(string channelName) where TData : new()
        {
            return await RedisChannelReader<TData>.SubscribeAsync(RedisConnectionFactory.GetConnection(_connectionString), channelName);
        }

        public IChannelWriter<TData> CreateChannelWriter<TData>(string channelName) where TData : new()
        {
            return new RedisChannelWriter<TData>(RedisConnectionFactory.GetConnection(_connectionString), channelName);
        }

    }
}
