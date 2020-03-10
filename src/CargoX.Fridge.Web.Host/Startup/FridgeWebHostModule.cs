using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CargoX.Fridge.Configuration;

namespace CargoX.Fridge.Web.Host.Startup
{
    [DependsOn(
       typeof(FridgeWebCoreModule))]
    public class FridgeWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public FridgeWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(FridgeWebHostModule).GetAssembly());
        }
    }
}
