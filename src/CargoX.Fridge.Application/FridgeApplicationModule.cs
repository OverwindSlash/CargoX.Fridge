using System;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using CargoX.Fridge.Authorization;
using CargoX.Fridge.Configuration;
using Microsoft.Extensions.Configuration;

namespace CargoX.Fridge
{
    [DependsOn(
        typeof(FridgeCoreModule), 
        typeof(AbpAutoMapperModule),
        typeof(AbpRedisCacheModule))]
    public class FridgeApplicationModule : AbpModule
    {
        private readonly IConfigurationRoot _configuration;

        public FridgeApplicationModule()
        {
            _configuration = AppConfigurations.Get(
                typeof(FridgeApplicationModule).GetAssembly().GetDirectoryPathOrNull());
        }

        public override void PreInitialize()
        {
            Configuration.Caching.UseRedis(options =>
            {
                options.ConnectionString = _configuration["Redis:ConnectionString"];
                string databaseIdStr = _configuration["Redis:DatabaseId"];
                if (Int32.TryParse(databaseIdStr, out var databaseId))
                {
                    options.DatabaseId = databaseId;
                }
                else
                {
                    options.DatabaseId = 0;
                }
            });

            //Configuration for all caches
            Configuration.Caching.ConfigureAll(cache =>
            {
                string expireStr = _configuration["Redis:ExpireHours"];
                if (double.TryParse(expireStr, out var expire))
                {
                    cache.DefaultSlidingExpireTime = TimeSpan.FromHours(expire);
                }
            });

            Configuration.Authorization.Providers.Add<FridgeAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(FridgeApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
