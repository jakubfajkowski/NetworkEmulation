using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkUtilities {
    public class Node {
        public delegate void MessageHandler(object sender, string text);

        private TcpListener _cloudTcpListener;
        private TcpClient _nodeTcpClient;
        private bool _online;
        protected int CableCloudListeningPort;
        protected CancellationTokenSource cts = new CancellationTokenSource();
        protected IPAddress IpAddress;

        public Node(string ipAddress, int cableCloudListeningPort, int cableCloudDataPort) {
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

        public event MessageHandler OnUpdateState;

        protected void UpdateState(string state) {
            OnUpdateState?.Invoke(this, state);
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
                ListenForNodeMessages(cts.Token);
                _online = true;
            });
        }

        private async void ListenForNodeMessages(CancellationToken ct) {
            using (var ns = _nodeTcpClient.GetStream()) {
                var buffer = new byte[CableCloudMessage.MaxByteBufferSize];

                while (!ct.IsCancellationRequested) {
                    var bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length, ct);
                    if (bytesRead <= 0)
                        break;

                    Recieve(CableCloudMessage.Deserialize(buffer));
                }
            }
        }

        protected virtual void Recieve(CableCloudMessage cableCloudMessage) {
            Debug.WriteLine("Recieved: " + cableCloudMessage);
        }


        protected void Send(CableCloudMessage cableCloudMessage) {
            try {
                var data = cableCloudMessage.Serialize();
                _nodeTcpClient.GetStream().Write(data, 0, data.Length);
            }
            catch {
                Debug.WriteLine("Sending ERROR!");
            }
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