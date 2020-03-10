using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CargoX.Fridge.Web.Host.SvcDiscovery
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSvcDiscovery(
            this IServiceCollection service,
            IConfigurationRoot configuration, 
            string svcDiscoveryName)
        {
            if (svcDiscoveryName.ToLower() == "nacos")
            {
                service.AddNacosAspNetCore(configuration);
            }

            return service;
        }
    }
}
