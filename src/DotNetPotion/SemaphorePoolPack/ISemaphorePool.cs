using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPotion.SemaphorePoolPack
{
    /// <summary>
    ///     Defines the contract for a pool of <see cref="SemaphoreSlim" /> instances identified by a key.
    /// </summary>
    public interface ISemaphorePool
    {
        /// <summary>
        ///     Asynchronously waits to enter the specified <see cref="SemaphoreSlim" /> instance.
        /// </summary>
        /// <param name="key">The key identifying the <see cref="SemaphoreSlim" /> instance.</param>
        /// <param name="timeout">An optional timeout value.</param>
        /// <param name="logTimeThreshold">An optional log threshold value.</param>
        /// <returns>
        ///     A task that represents the asynchronous wait operation. The task result is the <see cref="SemaphoreSlim" />
        ///     instance that was waited on.
        /// </returns>
        Task<SemaphoreSlim> WaitAsync(string key, TimeSpan? timeout = null, TimeSpan? logTimeThreshold = null);
    }
}