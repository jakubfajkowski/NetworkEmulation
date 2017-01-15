using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkUtilities.Serialization;

namespace NetworkUtilities.Network {
    class ConnectionComponent {
        public delegate void MessageHandler(object sender, string text);
        public event MessageHandler OnUpdateState;

        private TcpListener _connectionManagerTcpListener;
        private TcpClient _connectionObjectTcpClient;
        protected int ConnectionManagerListeningPort;
        protected IPAddress IpAddress;

        public bool Online { get; private set; }
        public int DataTransferPort { get; set; }

        protected ConnectionComponent(string connectionManagerIpAddress, int connectionManagerListeningPort, int dataTransferPort) {
            IpAddress = IPAddress.Parse(connectionManagerIpAddress);
            ConnectionManagerListeningPort = connectionManagerListeningPort;
            DataTransferPort = dataTransferPort;

            Initialize();
        }

        private void Initialize() {
            _connectionManagerTcpListener = CreateTcpListener(IpAddress, DataTransferPort);
            ListenForConnectRequest(_connectionManagerTcpListener);
            EstabilishConnection();
        }

        protected TcpListener CreateTcpListener(IPAddress ipAddress, int port) {
            TcpListener tcpListener = null;
            try {
                tcpListener = new TcpListener(ipAddress, port);
            }
            catch (Exception e) {
                Debug.Fail(e.ToString(),
                    $"Can't connect to port {DataTransferPort}!");
            }

            return tcpListener;
        }

        protected void ListenForConnectRequest(TcpListener tcpListener) {
            tcpListener.Start();
            Task.Run(async () => {
                _connectionObjectTcpClient = await tcpListener.AcceptTcpClientAsync();
                Online = true;
                ListenForMessages();
            });
        }

        private void ListenForMessages() {
            while (Online) {
                var cableCloudMessage = (CableCloudMessage)RecieveObject(_connectionObjectTcpClient.GetStream());
                Recieve(cableCloudMessage);
            }
        }

        protected void Recieve(object recievedObject) {
            throw new NotImplementedException();
        }


        protected void Send(CableCloudMessage cableCloudMessage) {
            SendObject(cableCloudMessage, _connectionObjectTcpClient.GetStream());
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

        private void EstabilishConnection() {
            var udpClient = new UdpClient();
            var bytesToSend = BinarySerializer.Serialize(DataTransferPort);
            var ipEndpoint = new IPEndPoint(IPAddress.Loopback, ConnectionManagerListeningPort);
            udpClient.Send(bytesToSend, bytesToSend.Length, ipEndpoint);
        }


        protected void UpdateState(string state) {
            OnUpdateState?.Invoke(this, state);
        }
    }
}
