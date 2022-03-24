using DistributedAsync.Abstractions;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedAsync.Redis
{
    class RedisChannelWriter<TData> : IChannelWriter<TData> where TData : new()
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly string _channelName;
        private ISubscriber Subscriber => _connection.GetSubscriber();

        public RedisChannelWriter(ConnectionMultiplexer connection, string channelName)
        {
            _connection = connection;
            _channelName = channelName;
        }

        public void Write(TData item)
        {
            var json = JsonConvert.SerializeObject(item);
            Subscriber.Publish(_channelName, json);
        }

        public async Task WriteAsync(TData item, CancellationToken cancellationToken = default)
        {
            var json = JsonConvert.SerializeObject(item);
            await Subscriber.PublishAsync(_channelName, json);
        }

    }
}
