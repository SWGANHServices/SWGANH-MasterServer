using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWGANH_MasterServer
{
    public class ConfigurationService
    {
        public ServiceProvider ServiceProvider { get; private set; }

        public static ConfigurationService CreateInstance()
        {
            return CreateInstance((s) => { });
        }

        public static ConfigurationService CreateInstance(Action<IServiceCollection> handler)
        {
            var instance = new ConfigurationService();

            var descriptors = CreateDefaultServiceDescriptors();
            handler(descriptors);

            instance.ServiceProvider = descriptors.BuildServiceProvider();
            return instance;
        }

        private static IServiceCollection CreateDefaultServiceDescriptors()
        {
            var Configuration = new ConfigurationBuilder()
                .AddJsonFile("Config.json")
                .Build();
            IServiceCollection serviceDescriptors = new ServiceCollection();
            serviceDescriptors.AddLogging(b => b.AddNLog());
            serviceDescriptors.AddSingleton<IConfigurationRoot>(Configuration);

            return serviceDescriptors;

        }
    }
}
