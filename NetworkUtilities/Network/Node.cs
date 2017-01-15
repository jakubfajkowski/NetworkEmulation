using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkUtilities.Log;
using NetworkUtilities.Serialization;

namespace NetworkUtilities.Network {
    public abstract class Node : LogObject {
        private TcpListener _cloudTcpListener;
        private TcpClient _nodeTcpClient;
        private bool _online;
        protected int CableCloudListeningPort;
        protected IPAddress IpAddress;

        protected Node(string ipAddress, int cableCloudListeningPort, int cableCloudDataPort) {
            IpAddress = IPAddress.Parse(ipAddress);
            CableCloudListeningPort = cableCloudListeningPort;
            CableCloudDataPort = cableCloudDataPort;
            Initialize();
        }

        public int CableCloudDataPort { get; protected set; }


        private void Initialize() {
            _cloudTcpListener = CreateTcpListener(IpAddress, CableCloudDataPort);
            ListenForConnectRequest(_cloudTcpListener);
            ConnectToCloud();
        }

        protected TcpListener CreateTcpListener(IPAddress ipAddress, int port) {
            TcpListener tcpListener = null;
            try {
                tcpListener = new TcpListener(ipAddress, port);
            }
            catch (Exception e) {
                Debug.Fail(e.ToString(),
                    $"Can't connect to port {CableCloudDataPort}!");
            }

            return tcpListener;
        }

        protected void ListenForConnectRequest(TcpListener tcpListener) {
            tcpListener.Start();
            Task.Run(async () => {
                _nodeTcpClient = await tcpListener.AcceptTcpClientAsync();
                _online = true;
                ListenForNodeMessages();
            });
        }

        private void ListenForNodeMessages() {
            while (Online()) {
                var cableCloudMessage = (CableCloudMessage) RecieveObject(_nodeTcpClient.GetStream());
                Recieve(cableCloudMessage);
            }
        }

        protected abstract void Recieve(CableCloudMessage cableCloudMessage);


        protected void Send(CableCloudMessage cableCloudMessage) {
            SendObject(cableCloudMessage, _nodeTcpClient.GetStream());
        }

        protected void SendObject(object objectToSend, Stream networkStream) {
            BinarySerializer.SerializeToStream(objectToSend, networkStream);
        }

        protected object RecieveObject(Stream networkStream) {
            return BinarySerializer.DeserializeFromStream(networkStream);
        }

        public List<AtmCell> AtmCells(CableCloudMessage cableCloudMessage) {
            var atmCells = BinarySerializer.Deserialize(cableCloudMessage.Data) as List<AtmCell>;
            return atmCells?.FindAll(cell => cell.Valid());
        }

        private void ConnectToCloud() {
            var udpClient = new UdpClient();
            var bytesToSend = BinarySerializer.Serialize(CableCloudDataPort);
            var ipEndpoint = new IPEndPoint(IPAddress.Loopback, CableCloudListeningPort);
            udpClient.Send(bytesToSend, bytesToSend.Length, ipEndpoint);
        }

        public bool Online() {
            return _online;
        }
    }
}