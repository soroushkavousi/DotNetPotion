using Microsoft.Extensions.DependencyInjection;

namespace DotNetPotion.ScopedTaskRunnerPack
{
    public static class ServiceInstaller
    {
        public static void AddScopedTaskRunner(this IServiceCollection services)
        {
            services.AddSingleton<IScopedTaskRunner, ScopedTaskRunner>();
        }
    }
}