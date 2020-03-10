using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace CargoX.Fridge.Web.Host.SvcDiscovery
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseSvcDiscovery(
            this IApplicationBuilder app,
            IConfigurationRoot configuration,
            string svcDiscoveryName
        )
        {
            //var lifetime = app.ApplicationServices.GetService(typeof(IHostApplicationLifetime));
            //var config = configuration;
            app.UseNacosAspNetCore();

            var hostname = Dns.GetHostName();

            IPHostEntry ipHost = Dns.GetHostEntry(hostname);

            return app;
        }
    }

    public class ServiceEntity
    {
        public string ServiceIP { get; set; }
        public int ServicePort { get; set; }
        public string ServiceName { get; set; }
        public string DiscoveryIP { get; set; }
        public int DiscoveryPort { get; set; }
    }
}
