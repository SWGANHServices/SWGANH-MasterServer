using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using Microsoft.Extensions.Logging;
using SWGANH_Core;
using SWGANH_Core.PackageParser;
using SWGANH_Core.PackageParser.PackageImplimentations;
using SWGANH_Core.Server;
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

        private List<NetworkConnection> ClientConnections = new List<NetworkConnection>();

        private TcpListener tcpListener;
        private Thread listenerWorkerThread;
        private Thread clientPackageReceiverThread;

        public bool Running { get; set; }
        public event Action<TcpClient> ClientConnected;

        public ILogger logger;
        private readonly IServiceProvider serviceProvider;
        private readonly IPackageParser packageParser;
        private readonly IServerPackageDispatcher packageDispatcher;

        public NetworkService(IConfigurationRoot config, ILogger<NetworkService> logger,
            IServiceProvider serviceProvider, IPackageParser packageParser,
            IServerPackageDispatcher packageDispatcher)
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

        private int recievedPackageInterationCounter = 0;
        private List<NetworkConnection> invalidConnections = new List<NetworkConnection>();

        private async void ReceivePackage()
        {
            try
            {
                logger.LogInformation("NetworkService.receivePackage thread sucessfully started");
                while(Running)
                {
                    await Task.Delay(1);
                    if(Running)
                    {
                        lock(AddRemoveLocker)
                        {
                            if(++recievedPackageInterationCounter == 1000)
                            {
                                recievedPackageInterationCounter = 0;
                                foreach(var client in ClientConnections)
                                {
                                    try
                                    {
                                        packageParser.ParserPackageToStream(new KeepAlivePackage(), client.Writer);
                                    }
                                    catch(Exception)
                                    {
                                        invalidConnections.Add(client);
                                        logger.LogInformation("KeepAlive Exception");
                                    }
                                }
                            }
                            if(invalidConnections.Count > 0)
                            {
                                foreach(var conn in invalidConnections)
                                {
                                    ClientConnections.Remove(conn);
                                }
                                invalidConnections.Clear();
                            }
                        }
                        var clientConArr = ClientConnections.ToArray();
                        foreach(var client in clientConArr)
                        {
                            if(client.AvailableBytes > 0)
                            {
                                logger.LogInformation($"Package from Client {client.ConnectionId}");
                                var package = packageParser.ParserPackageFromStream(client.Reader);
                                packageDispatcher.DispatchPackage(package, client);
                            }
                        }
                    }
                }
            }
            finally
            {
                logger.LogInformation("Server Stopped");
            }
        }

        public void Stop()
        {
            if(listenerWorkerThread == null)
            {
                return; // Always stop
            }
            Running = false;
            listenerWorkerThread.Abort();
            listenerWorkerThread = null;
            clientPackageReceiverThread.Abort();
            clientPackageReceiverThread = null;
            tcpListener.Stop();
        }

        protected virtual void OnClientConnected(TcpClient connection)
        {
            ClientConnected?.Invoke(connection);
            var client = new NetworkConnection(Guid.NewGuid(), connection);
            lock(AddRemoveLocker)
            {
                ClientConnections.Add(client);
            }
            logger.LogInformation($"New Client Connected Guid: {client.ConnectionId}");
        }

    }
}