using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CargoX.Fridge.Authorization;

namespace CargoX.Fridge
{
    [DependsOn(
        typeof(FridgeCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class FridgeApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
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
