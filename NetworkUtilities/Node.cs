using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetworkUtilities {
    public class Node {
        public static int CableCloudUdpPort = 10000;
        private readonly TcpListener _cloudTcpListener;
        private TcpClient _nodeTcpClient;
        private bool _online;

        public Node() {
            CableCloudTcpPort = RandomFreePort();
            _cloudTcpListener = CreateTcpListener(IPAddress.Loopback, CableCloudTcpPort);
            ListenForConnectRequest(_cloudTcpListener);
            ConnectToCloud();
        }

        public int CableCloudTcpPort { get; }

        protected int RandomFreePort() {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint) l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        protected TcpListener CreateTcpListener(IPAddress ipAddress, int port) {
            TcpListener tcpListener = null;
            try {
                tcpListener = new TcpListener(ipAddress, port);
            }
            catch (Exception e) {
                Debug.Fail(e.ToString(),
                    string.Format("Can't connect to port {0}!", CableCloudTcpPort));
            }

            return tcpListener;
        }

        protected void ListenForConnectRequest(TcpListener tcpListener) {
            tcpListener.Start();
            Task.Run(async () => {
                _nodeTcpClient = await tcpListener.AcceptTcpClientAsync();
                ListenForNodeMessages();
                _online = true;
            });
        }

        private void ListenForNodeMessages() {
            Task.Run(async () => {
                using (var ns = _nodeTcpClient.GetStream()) {
                    var buffer = new byte[CableCloudMessage.MaxByteBufferSize];

                    while (true) {
                        var bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead <= 0)
                            break;

                        HandleMessage(CableCloudMessage.Deserialize(buffer));
                    }
                }
            });
        }

        protected virtual void HandleMessage(CableCloudMessage cableCloudMessage) {
            Debug.WriteLine("Recieved: " + cableCloudMessage);
        }


        public void Send(byte[] data) {
            Task.Run(() => {
                try {
                    _nodeTcpClient.GetStream().Write(data, 0, data.Length);
                }
                catch {
                    Debug.WriteLine("Sending ERROR!");
                }
            });
        }

        private void ConnectToCloud() {
            var udpClient = new UdpClient();
            var bytesToSend = BinarySerializer.Serialize(CableCloudTcpPort);
            var ipEndpoint = new IPEndPoint(IPAddress.Loopback, CableCloudUdpPort);
            udpClient.Send(bytesToSend, bytesToSend.Length, ipEndpoint);
        }

        public bool IsOnline() {
            return _online;
        }
    }
}