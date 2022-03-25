using DistributedAsync.Abstractions;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedAsync.Redis
{
    class RedisChannelWriter<TData> : IChannelWriter<TData> where TData : new()
    {
        protected readonly ConnectionMultiplexer _connection;
        protected readonly string _channelName;
        protected ISubscriber Subscriber => _connection.GetSubscriber();
        private readonly ISerializer _serializer;

        public RedisChannelWriter(ConnectionMultiplexer connection, string channelName, ISerializer serializer)
        {
            _connection = connection;
            _channelName = channelName;
            _serializer = serializer;
        }

        public virtual void Write(TData item)
        {
            var data = Serilize(item);
            Subscriber.Publish(_channelName.NormalizeChannelName(), data);
        }

        public virtual async Task WriteAsync(TData item, CancellationToken cancellationToken = default)
        {
            var data = Serilize(item);
            await Subscriber.PublishAsync(_channelName.NormalizeChannelName(), data);
        }
        protected string Serilize(object value) => _serializer.Serialize(value);

    }
}
