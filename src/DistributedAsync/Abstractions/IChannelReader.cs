using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedAsync.Abstractions
{
    public interface IChannelReader<TData> : IDisposable where TData : new()
    {
        event Action<TData> ReadCompleted;
        TData Read();
        TData Read(int millisecondsTimeout);
        Task<TData> ReadAsync(CancellationToken cancellationToken = default);
    }
}
