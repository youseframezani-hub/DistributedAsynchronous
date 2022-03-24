using System.Threading;
using System.Threading.Tasks;

namespace DistributedAsync.Abstractions
{
    public interface IChannelWriter<TData> where TData : new()
    {
        void Write(TData item);
        Task WriteAsync(TData item, CancellationToken cancellationToken = default);
    }
}
