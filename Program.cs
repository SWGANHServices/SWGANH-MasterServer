using Microsoft.Extensions.DependencyInjection;
using System;
using SWGANH_Core.PackageParser;
using SWGANH_MasterServer.Services;
using SWGANH_MasterServer.Business;

namespace SWGANH_MasterServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var configServices = ConfigurationService.CreateInstance(serviceDescriptors =>
            {
                serviceDescriptors.AddSingleton<IPackageParser, PackageParser>();
                serviceDescriptors.AddSingleton<IPackageDispatcher, PackageDispatcher>();

                serviceDescriptors.AddScoped<ServerConnectionHandler>();
                serviceDescriptors.AddSingleton<NetworkService>();

                serviceDescriptors.AddScoped<IUserRepository, UserRepository>();

            });

            configServices.ServiceProvider.GetRequiredService<NetworkService>().Start();
            configServices.ServiceProvider.GetRequiredService<IPackageDispatcher>().Start();
            Console.ReadLine();
        }
    }
}
