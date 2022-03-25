using DistributedAsync.Abstractions;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedAsync.Redis
{
    class RedisChannelReader<TData> : IChannelReader<TData> where TData : new()
    {
        private TData _data;
        private readonly EventWaitHandle _waitHandle;
        private readonly ISerializer _serializer;
        protected readonly string _channelName;
        protected readonly ConnectionMultiplexer _connection;
        private ISubscriber Subscriber => _connection.GetSubscriber();

        public event Action<TData> ReadCompleted;

        protected RedisChannelReader(ConnectionMultiplexer connection, string channelName, ISerializer serializer)
        {
            _data = default;
            _connection = connection;
            _channelName = channelName;
            _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            _serializer = serializer;
        }
        public static async Task<RedisChannelReader<TData>> SubscribeAsync(ConnectionMultiplexer connection, ISerializer serializer, string channelName)
        {
            var channelReader = new RedisChannelReader<TData>(connection, channelName, serializer);
            await channelReader.SubscribeAsync();
            return channelReader;
        }
        public static RedisChannelReader<TData> Subscribe(ConnectionMultiplexer connection, ISerializer serializer, string channelName)
        {
            var channelReader = new RedisChannelReader<TData>(connection, channelName, serializer);
            channelReader.Subscribe();
            return channelReader;
        }
        public virtual TData Read()
        {
            _waitHandle.WaitOne();
            return _data;
        }
        public virtual TData Read(int millisecondsTimeout)
        {
            _waitHandle.WaitOne(millisecondsTimeout);
            return _data;
        }
        public virtual async Task<TData> ReadAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(Read, cancellationToken);
        }
        public virtual void Dispose()
        {
            Subscriber.Unsubscribe(_channelName.NormalizeChannelName());
            _waitHandle.Dispose();
        }
        protected TData Deserialize(string value) => _serializer.Deserialize<TData>(value);
        protected async Task SubscribeAsync()
        {
            await Subscriber.SubscribeAsync(_channelName.NormalizeChannelName(), Consumer);
        }
        protected void Subscribe()
        {
            Subscriber.Subscribe(_channelName.NormalizeChannelName(), Consumer);
        }
        private void Consumer(RedisChannel channel, RedisValue value)
        {
            _data = Deserialize(value);
            ReadCompleted?.Invoke(_data);
            _waitHandle.Set();
        }

    }
}
