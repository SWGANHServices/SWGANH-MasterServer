using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using Microsoft.Extensions.Logging;
using SWGANH_Core.PackageParser;
using SWGANH_MasterServer.Service.ServiceModels;
using SWGANH_MasterServer.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SWGANH_MasterServer
{
    internal class NetworkService
    {
        readonly object AddRemoveLocker = new object();

        private List<ClientConnection> ClientConnections = new List<ClientConnection>();

        private TcpListener tcpListener;
        private Thread listenerWorkerThread;
        private Thread clientPackageReceiverThread;

        public bool Running { get; set; }
        public event Action<TcpClient> ClientConnected;

        public ILogger logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IPackageParser packageParser;
        private readonly IPackageDispatcher packageDispatcher;

        public NetworkService(IConfigurationRoot config, ILogger<NetworkService> logger,
            IServiceProvider serviceProvider, IPackageParser packageParser,
            IPackageDispatcher packageDispatcher)
        {
            tcpListener = new TcpListener(IPAddress.Parse(config.GetValue<string>("host")),
                config.GetValue<int>("port"));
            Running = false;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.packageParser = packageParser;
            this.packageDispatcher = packageDispatcher;
        }

        public NetworkService(IPAddress address, int port,
            ILogger<NetworkService> logger, IServiceProvider serviceProvider) 
        {
            tcpListener = new TcpListener(address, port);
            Running = false;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public void Start()
        {
            if(listenerWorkerThread != null && listenerWorkerThread.ThreadState == ThreadState.Running)
            {
                return; // still running
            }

            listenerWorkerThread = new Thread(async () =>
            {
                Running = true;
                tcpListener.Start();
                logger.LogInformation("Network Service Start Sucessful..");
                try
                {
                    while(Running)
                    {
                        await Task.Delay(100);
                        if(Running && tcpListener.Pending())
                        {
                            OnClientConnected(await tcpListener.AcceptTcpClientAsync());
                        }
                    }
                }
                finally
                {
                    logger.LogInformation("Server Stopped!");
                }
            })
            {
                IsBackground = true
            };
            listenerWorkerThread.Start();
            clientPackageReceiverThread = new Thread(ReceivePackage)
            {
                IsBackground = true,
            };
            clientPackageReceiverThread.Start();
        }
    }
}