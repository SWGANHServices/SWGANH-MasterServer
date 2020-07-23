using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SWGANH_Core.PackageParser;
using SWGANH_MasterServer.Service.ServiceModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SWGANH_MasterServer.Services
{
    public class PackageDispatcher : IPackageDispatcher
    {
        private readonly ILogger<PackageDispatcher> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IPackageParser packageParser;
        private BlockingCollection<Tuple<ClientConnection, PackageBase>> queue
            = new BlockingCollection<Tuple<ClientConnection, PackageBase>>();
        public bool Running { get; private set; }

        Thread dispatcherThread;

        public PackageDispatcher(ILogger<PackageDispatcher> logger, IServiceProvider serviceProvider, IPackageParser packageParser)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.packageParser = packageParser;
        }

        public void Start()
        {
            dispatcherThread = new Thread(() =>
            {
                logger.LogInformation("Dispacter started...");
                Running = true;
                try
                {
                    while (Running)
                    {
                        if (queue.TryTake(out var item))
                        {
                            using (var scope = serviceProvider.CreateScope())
                            {
                                var (connection, package) = item;
                                var connHandler = scope.ServiceProvider.GetRequiredService<ServerConnectionHandler>();
                                connHandler.InvokeAction(connection, package, package.ID);
                            }
                        }
                    }
                }
                finally
                {
                    logger.LogError("Dispatcher Stoped...");
                }
            })
            { IsBackground = true, };
            dispatcherThread.Start();
        }

        public void DispatchPackage(PackageBase package, ClientConnection connection)
        {
            queue.Add(new Tuple<ClientConnection, PackageBase>(connection, package));
        }
    }
}
