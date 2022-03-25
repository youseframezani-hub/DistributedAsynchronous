using DistributedAsync.Abstractions;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedAsync.Redis
{
    class PersistenceRedisChannelReader<TData> : RedisChannelReader<TData> where TData : new()
    {

        private readonly int? dbNum;
        private IDatabase Database => dbNum == null ? _connection.GetDatabase() : _connection.GetDatabase(dbNum.Value);

        private PersistenceRedisChannelReader(ConnectionMultiplexer connection, string channelName, ISerializer serializer, int? dbNum) : base(connection, channelName, serializer)
        {
            this.dbNum = dbNum;
        }
        public override TData Read()
        {
            if (TryGetData(out var data))
                return data;
            return base.Read();
        }
        public override TData Read(int millisecondsTimeout)
        {
            if (TryGetData(out var data))
                return data;
            return base.Read(millisecondsTimeout);
        }
        public override async Task<TData> ReadAsync(CancellationToken cancellationToken = default)
        {
            var data = await Database.StringGetAsync(_channelName.NormalizeKeyName());
            if (data.HasValue)
                return Deserialize(data);
            return await base.ReadAsync(cancellationToken);
        }

        public static async Task<PersistenceRedisChannelReader<TData>> SubscribeAsync(ConnectionMultiplexer connection, ISerializer serializer, string channelName, int? dbNum)
        {
            var channelReader = new PersistenceRedisChannelReader<TData>(connection, channelName, serializer, dbNum);
            await channelReader.SubscribeAsync();
            return channelReader;
        }
        public static PersistenceRedisChannelReader<TData> Subscribe(ConnectionMultiplexer connection, ISerializer serializer, string channelName, int? dbNum)
        {
            var channelReader = new PersistenceRedisChannelReader<TData>(connection, channelName, serializer, dbNum);
            channelReader.Subscribe();
            return channelReader;
        }
        public override void Dispose()
        {
            base.Dispose();
            Database.KeyDelete(_channelName.NormalizeKeyName());
        }

        private bool TryGetData(out TData data)
        {
            data = default;
            var value = Database.StringGet(_channelName.NormalizeKeyName());
            if (value.HasValue)
                data = Deserialize(value);
            return value.HasValue;
        }
    }
}
