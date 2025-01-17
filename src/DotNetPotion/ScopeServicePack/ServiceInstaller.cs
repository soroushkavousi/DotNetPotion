using Microsoft.Extensions.DependencyInjection;

namespace DotNetPotion.ScopeServicePack
{
    public static class ServiceInstaller
    {
        public static void AddScopeService(this IServiceCollection services)
        {
            services.AddSingleton<IScopeService, ScopeService>();
        }
    }
}