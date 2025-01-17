using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetPotion.SemaphorePoolPack
{
    /// <summary>
    ///     Provides extension methods for registering the <see cref="SemaphorePool" /> service with the dependency injection
    ///     container.
    /// </summary>
    public static class ServiceInstaller
    {
        /// <summary>
        ///     Adds the <see cref="SemaphorePool" /> service to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The service collection to add the <see cref="SemaphorePool" /> service to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddSemaphorePool(this IServiceCollection services)
        {
            services.AddSingleton<ISemaphorePool, SemaphorePool>();
            return services;
        }

        /// <summary>
        ///     Adds the <see cref="SemaphorePool" /> service to the specified <see cref="IServiceCollection" /> with configuration
        ///     options.
        /// </summary>
        /// <param name="services">The service collection to add the <see cref="SemaphorePool" /> service to.</param>
        /// <param name="configureOptions">An action to configure the <see cref="SemaphorePoolOptions" />.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddSemaphorePool(this IServiceCollection services, Action<SemaphorePoolOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<ISemaphorePool, SemaphorePool>();
            return services;
        }
    }
}