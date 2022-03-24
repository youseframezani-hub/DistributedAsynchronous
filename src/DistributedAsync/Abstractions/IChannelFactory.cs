using System.Threading.Tasks;

namespace DistributedAsync.Abstractions
{
    public interface IChannelFactory
    {
        IChannelWriter<TData> CreateChannelWriter<TData>(string channelName) where TData : new();
        IChannelReader<TData> CreateChannelReader<TData>(string channelName) where TData : new();
        Task<IChannelReader<TData>> CreateChannelReaderAsync<TData>(string channelName) where TData : new();
    }
}
