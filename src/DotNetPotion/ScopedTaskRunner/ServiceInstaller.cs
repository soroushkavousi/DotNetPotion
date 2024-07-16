using Microsoft.Extensions.DependencyInjection;

namespace DotNetPotion.ScopedTaskRunner
{
    public static class ServiceInstaller
    {
        public static IServiceCollection AddScopedTaskRunner(this IServiceCollection services)
        {
            services.AddSingleton<IScopedTaskRunner, ScopedTaskRunner>();
            return services;
        }
    }
}