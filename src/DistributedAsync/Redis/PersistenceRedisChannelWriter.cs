using DistributedAsync.Abstractions;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedAsync.Redis
{
    class PersistenceRedisChannelWriter<TData> : RedisChannelWriter<TData> where TData : new()
    {
        private readonly int? dbNum;
        private readonly TimeSpan _expirtionTime;
        private IDatabase Database => dbNum == null ? _connection.GetDatabase() : _connection.GetDatabase(dbNum.Value);

        public PersistenceRedisChannelWriter(ConnectionMultiplexer connection, string channelName, ISerializer serializer, int? dbNum = null, TimeSpan? expirtionTime = null)
            : base(connection, channelName, serializer)
        {
            this.dbNum = dbNum;
            _expirtionTime = expirtionTime ?? TimeSpan.FromHours(6);
        }

        public override void Write(TData item)
        {
            var data = Serilize(item);
            Subscriber.Publish(_channelName.NormalizeChannelName(), data);
            Database.StringSet(_channelName.NormalizeKeyName(), data, _expirtionTime);
        }

        public override async Task WriteAsync(TData item, CancellationToken cancellationToken = default)
        {
            var data = Serilize(item);
            await Subscriber.PublishAsync(_channelName.NormalizeChannelName(), data);
            await Database.StringSetAsync(_channelName.NormalizeKeyName(), data, _expirtionTime);
        }

    }
}
