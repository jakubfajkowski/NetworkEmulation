using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkUtilities;

namespace NetworkEmulation {
    public class CableCloud {
        private readonly UdpClient _connectionUdpClient;
        private readonly Dictionary<SocketNodePortPair, SocketNodePortPair> _linkDictionary;
        private readonly Dictionary<int, TcpClient> _nodesTcpClients;
        public bool Online { get; private set; }

        public CableCloud() {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 10000);
            _connectionUdpClient = new UdpClient(ipEndPoint);

            _nodesTcpClients = new Dictionary<int, TcpClient>();
            _linkDictionary = new Dictionary<SocketNodePortPair, SocketNodePortPair>();

            ListenForConnectionRequests();
        }

        private void ListenForConnectionRequests() {
            Task.Run(async () => {
                using (_connectionUdpClient) {
                    Online = true;
                    while (true) {
                        var receivedData = await _connectionUdpClient.ReceiveAsync();
                        EstabilishNodeConnection(BitConverter.ToInt32(receivedData.Buffer, 0));
                    }
                }
            });
        }

        private void EstabilishNodeConnection(int port) {
            var nodeTcpClient = new TcpClient();
            try {
                nodeTcpClient.Connect(IPAddress.Loopback, port);
                _nodesTcpClients.Add(port, nodeTcpClient);
                Console.WriteLine("Connected to Node on port: " + port);
                ListenForNodeMessages(nodeTcpClient, port);
            }
            catch (SocketException e) {
                Console.WriteLine(e.Message);
            }
        }

        private void ListenForNodeMessages(TcpClient nodeTcpClient, int inputPort) {
            Task.Run(async () => {
                using (var ns = nodeTcpClient.GetStream()) {
                    var buffer = new byte[CableCloudMessage.MaxByteBufferSize];

                    while (true) {
                        var bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead <= 0)
                            break;

                        var cableCloudMessage = CableCloudMessage.deserialize(buffer);
                        Console.WriteLine("Router " + inputPort + ": " + cableCloudMessage.portNumber + " - message recieved.");
                        var input = new SocketNodePortPair(cableCloudMessage.portNumber, inputPort);
                        var output = LookUpLinkDictionary(input);
                        cableCloudMessage.portNumber = output.nodePortNumber;

                        PassCableCloudMessage(cableCloudMessage, output.socketPortNumber);
                    }
                }
            });
        }

        private SocketNodePortPair LookUpLinkDictionary(SocketNodePortPair input) {
            return _linkDictionary[input];
        }

        private void PassCableCloudMessage(CableCloudMessage cableCloudMessage, int outputPort) {
            try {
                var tcpClient = _nodesTcpClients[outputPort];

                sendBytes(CableCloudMessage.serialize(cableCloudMessage), tcpClient);
                Console.WriteLine("Router " + outputPort + ": " + cableCloudMessage.portNumber + " - message sent.");
            }
            catch (KeyNotFoundException e) {
                Console.WriteLine("No avaliable link.");
            }
        }

        private void sendBytes(byte[] data, TcpClient tcpClient) {
            tcpClient.GetStream().Write(data, 0, data.Length);
        }

        public void AddLink(SocketNodePortPair key, SocketNodePortPair value) {
            _linkDictionary.Add(key, value);
        }

        public void RemoveLink(SocketNodePortPair key) {
            _linkDictionary.Remove(key);
        }
    }
}