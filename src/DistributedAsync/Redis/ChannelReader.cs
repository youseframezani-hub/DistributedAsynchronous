using DistributedAsync.Abstractions;
using Newtonsoft.Json;
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
        private readonly string _channelName;
        private readonly ConnectionMultiplexer _connection;
        private ISubscriber Subscriber => _connection.GetSubscriber();
        public event Action<TData> ReadCompleted;

        private RedisChannelReader(ConnectionMultiplexer connection, string channelName)
        {
            _data = default;
            _connection = connection;
            _channelName = channelName;
            _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        }
        public static async Task<RedisChannelReader<TData>> SubscribeAsync(ConnectionMultiplexer connection, string channelName)
        {
            var channelReader = new RedisChannelReader<TData>(connection, channelName);
            await channelReader.SubscribeAsync();
            return channelReader;
        }
        public static RedisChannelReader<TData> Subscribe(ConnectionMultiplexer connection, string channelName)
        {
            var channelReader = new RedisChannelReader<TData>(connection, channelName);
            channelReader.Subscribe();
            return channelReader;
        }
        public TData Read()
        {
            _waitHandle.WaitOne();
            return _data;
        }
        public TData Read(int millisecondsTimeout)
        {
            _waitHandle.WaitOne(millisecondsTimeout);
            return _data;
        }
        public async Task<TData> ReadAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(Read, cancellationToken);
        }
        public void Dispose()
        {
            Subscriber.Unsubscribe(_channelName);
            _waitHandle.Dispose();
        }
        private async Task SubscribeAsync()
        {
            await Subscriber.SubscribeAsync(_channelName, Consumer);
        }
        private void Subscribe()
        {
            Subscriber.Subscribe(_channelName, Consumer);
        }
        private void Consumer(RedisChannel channel, RedisValue value)
        {
            _data = JsonConvert.DeserializeObject<TData>(value);
            ReadCompleted?.Invoke(_data);
            _waitHandle.Set();
        }

    }
}
