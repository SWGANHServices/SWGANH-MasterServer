using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace SWGANH_MasterServer.Service.ServiceModels
{
    public class ClientConnection
    {
        public Guid ConnectionId { get; set; }

        private TcpClient tcpClient;

        private readonly NetworkStream stream;

        public BinaryReader Reader { get; }
        public BinaryWriter Writer { get; }

        public ClientConnection(Guid ConnectionId, TcpClient tcpClient)
        {
            this.ConnectionId = ConnectionId;
            this.tcpClient = tcpClient;
            this.stream = tcpClient.GetStream();
            this.Reader = new BinaryReader(stream);
            this.Writer = new BinaryWriter(stream);
        }

        public int AvailableBytes => tcpClient.Available;
        public bool isConnected => tcpClient.Connected;
    }
}
