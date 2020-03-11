using System;
using System.Net;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CargoX.Fridge.Web.Host.SvcDiscovery
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseSvcDiscovery(
            this IApplicationBuilder app,
            IConfigurationRoot configuration,
            string svcDiscoveryName)
        {
            //var lifetime = app.ApplicationServices.GetService(typeof(IHostApplicationLifetime));
            //var config = configuration;
            app.UseNacosAspNetCore();

            return app;
        }

        public static IApplicationBuilder RegisterConsul(
            this IApplicationBuilder app,
            IHostApplicationLifetime lifetime,
            ServiceEntity serviceEntity)
        {
            var consulClient = new ConsulClient(x =>
                x.Address = new Uri($@"http://{serviceEntity.DiscoveryIP}:{serviceEntity.DiscoveryPort}"));

            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                Interval = TimeSpan.FromSeconds(10),
                HTTP = $"http://{serviceEntity.ServiceName}:{serviceEntity.ServicePort}/api/health",
                Timeout = TimeSpan.FromSeconds(5)
            };

            // Register service with consul
            var registration = new AgentServiceRegistration()
            {
                Checks = new[] { httpCheck },
                ID = Guid.NewGuid().ToString(),
                Name = serviceEntity.ServiceName,
                Address = serviceEntity.ServiceIP,
                Port = serviceEntity.ServicePort,
                Tags = new[] { $"urlprefix-/{serviceEntity.ServiceName}" }
            };

            consulClient.Agent.ServiceRegister(registration).Wait();
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

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
