using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPotion.SemaphorePool
{
    /// <summary>
    /// Provides a pool of <see cref="SemaphoreSlim"/> instances identified by a key.
    /// Allows asynchronous waiting with configurable timeout and logging thresholds.
    /// </summary>
    public class SemaphorePool : ISemaphorePool
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

        private readonly ILogger<SemaphorePool> _logger;
        private readonly SemaphorePoolOptions _options;

        public SemaphorePool(ILogger<SemaphorePool> logger, IOptions<SemaphorePoolOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        /// <summary>
        /// Asynchronously waits to enter the specified <see cref="SemaphoreSlim"/> instance.
        /// If the timeout is reached, it will create a new semaphore to wait and retry.
        /// </summary>
        /// <param name="key">The key identifying the <see cref="SemaphoreSlim"/> instance.</param>
        /// <param name="timeout">An optional timeout value, defaulting to the configured default timeout if not specified.</param>
        /// <param name="logTimeThreshold">An optional log threshold value, defaulting to the configured default log threshold if not specified.</param>
        /// <returns>A task that represents the asynchronous wait operation. The task result is the <see cref="SemaphoreSlim"/> instance that was waited on.</returns>
        public async Task<SemaphoreSlim> WaitAsync(string key, TimeSpan? timeout = null, TimeSpan? logTimeThreshold = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (timeout is null)
                    timeout = _options.DefaultTimeout;

                if (logTimeThreshold is null)
                    logTimeThreshold = _options.DefaultLogTimeThreshold;

                SemaphoreSlim semaphore = _semaphores.GetOrAdd(key, new SemaphoreSlim(1, 1));
                bool entered = await semaphore.WaitAsync(timeout.Value);
                if (!entered)
                {
                    _logger.LogCritical("SemaphorePool: Failed to enter semaphore with key {SemaphoreKey}" +
                        " within a timeout of {Timeout}", key, timeout.Value);
                    _semaphores.TryRemove(key, out SemaphoreSlim _);
                    return await WaitAsync(key, timeout);
                }
                return semaphore;
            }
            finally
            {
                sw.Stop();
                if (sw.Elapsed >= logTimeThreshold)
                    _logger.LogWarning("SemaphorePool: WaitAsync took too long." +
                        " ExecutionTime: {ExecutionTime:0.###}s - SemaphoreKey: {SemaphoreKey}" +
                        " - Timeout: {Timeout}", sw.Elapsed.TotalSeconds, key, logTimeThreshold);
            }
        }
    }
}