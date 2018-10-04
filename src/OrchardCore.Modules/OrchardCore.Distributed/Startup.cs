using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Distributed.Drivers;
using OrchardCore.Distributed.Messaging;
using OrchardCore.Distributed.Options;
using OrchardCore.Environment.Cache;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Settings;

namespace OrchardCore.Distributed
{
    /// <summary>
    /// These services are registered on the tenant service collection
    /// </summary>
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DistributedSignal>();
            services.AddSingleton<ISignal>(sp => sp.GetRequiredService<DistributedSignal>());
            services.AddSingleton<IModularTenantEvents>(sp => sp.GetRequiredService<DistributedSignal>());
        }
    }

    [Feature("OrchardCore.Distributed.Redis.Settings")]
    public class RedisSettingsStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<IDisplayDriver<ISite>, RedisSiteSettingsDisplayDriver>();

            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<RedisOptions>, RedisOptionsSetup>());
        }
    }

    [Feature("OrchardCore.Distributed.Redis.Cache")]
    public class DistributedRedisCacheStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<RedisCacheOptions>, RedisCacheOptionsSetup>());

            services.AddDistributedRedisCache(o => { });
        }
    }

    [Feature("OrchardCore.Distributed.Redis.MessageBus")]
    public class RedisSignalStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMessageBus, RedisMessageBus>();
        }
    }
}
