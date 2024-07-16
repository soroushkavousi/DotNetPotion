using System;

namespace DotNetPotion.SemaphorePool
{
    /// <summary>
    /// Represents the configuration options for the <see cref="ISemaphorePool"/>.
    /// </summary>
    public class SemaphorePoolOptions
    {
        /// <summary>
        /// Gets or sets the default timeout value for waiting to enter a semaphore.
        /// If the timeout is reached, SemaphorePool will attempt to wait with a new semaphore.
        /// </summary>
        public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets or sets the default threshold time for logging long waits.
        /// </summary>
        public TimeSpan DefaultLogTimeThreshold { get; set; } = TimeSpan.FromSeconds(5);
    }
}