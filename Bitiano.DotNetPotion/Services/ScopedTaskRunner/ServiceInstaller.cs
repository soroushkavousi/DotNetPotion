using Microsoft.Extensions.DependencyInjection;

namespace Bitiano.DotNetPotion.Services.ScopedTaskRunner;

public static class ServiceInstaller
{
    public static IServiceCollection AddScopedTaskRunner(this IServiceCollection services)
    {
        services.AddSingleton<IScopedTaskRunner, ScopedTaskRunner>();
        return services;
    }
}