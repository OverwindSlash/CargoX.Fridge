using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace CargoX.Fridge.Web.Host.SvcDiscovery
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseSvcDiscovery(
            this IApplicationBuilder app,
            IConfigurationRoot configuration,
            string svcDiscoveryName)
        {
            if (svcDiscoveryName.ToLower() == "nacos")
            {
                app.UseNacosAspNetCore();
            }

            if (svcDiscoveryName.ToLower() == "consul")
            {
                Uri endPoint = new Uri(configuration["App:ServerRootAddress"]);

                ServiceEntity se = new ServiceEntity()
                {
                    ServiceIP = endPoint.Host,
                    ServicePort = endPoint.Port,
                    ServiceName = configuration["SvcDisco:ServiceName"],
                    DiscoveryIP = configuration["SvcDisco:DiscoveryIp"],
                    DiscoveryPort = Int32.Parse(configuration["SvcDisco:DiscoveryPort"]),
                    TimeOut = Int32.Parse(configuration["SvcDisco:TimeOut"]),
                    Interval = Int32.Parse(configuration["SvcDisco:Interval"])
                };

                app.RegisterConsul(se);
            }

            return app;
        }

        public static IApplicationBuilder RegisterConsul(
            this IApplicationBuilder app,
            ServiceEntity serviceEntity)
        {
            IHostApplicationLifetime lifetime = 
                app.ApplicationServices.GetService(typeof(IHostApplicationLifetime)) as IHostApplicationLifetime;

            var consulClient = new ConsulClient(x =>
                x.Address = new Uri($@"http://{serviceEntity.DiscoveryIP}:{serviceEntity.DiscoveryPort}"));

            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(serviceEntity.TimeOut),
                Interval = TimeSpan.FromSeconds(serviceEntity.Interval),
                HTTP = $"http://{serviceEntity.ServiceIP}:{serviceEntity.ServicePort}/api/health",
                Timeout = TimeSpan.FromSeconds(serviceEntity.TimeOut)
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
        public int TimeOut { get; set; }
        public int Interval { get; set; }
    }
}
