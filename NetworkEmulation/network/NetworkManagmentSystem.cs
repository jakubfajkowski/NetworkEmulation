using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkEmulation.Editor.Element;
using NetworkEmulation.Log;

namespace NetworkEmulation.Network {
    public class NetworkManagmentSystem : LogObject {
        private const int ListenUdpPort = 6666;
        private const int MaxTimeNotReceivingKeepAliveMessage = 2000;
        private readonly List<ConnectionTableRow> _connectionTable;
        private readonly Dictionary<int, DateTime> _keepAliveDictionary;
        private readonly UdpClient _listenUdpClient;
        private readonly Thread _messageThread;

        //private bool isKeepAliveListenerActive;
        //private bool isCheckKeepAliveTableActive;
        //private bool isListenForConnectionActive;

        private readonly List<string> _receivedMessagesList;
        private List<string> _receivedConnectionLabelsList;


        public NetworkManagmentSystem() {
            _keepAliveDictionary = new Dictionary<int, DateTime>();
            _connectionTable = new List<ConnectionTableRow>();

            _receivedMessagesList = new List<string>();
            _receivedConnectionLabelsList = new List<string>();

            var ipEndPoint = new IPEndPoint(IPAddress.Any, ListenUdpPort);
            _listenUdpClient = new UdpClient(ipEndPoint);

            ListenForConnectionRequests();

            _messageThread = new Thread(RunThread);
            _messageThread.Start();

            var checkKeepAliveTableThread = new Thread(checkKeepAliveTable);
            checkKeepAliveTableThread.Start();
        }


        public ConnectionTableRow GetRow(int node1, int node2) {
            foreach (var row in _connectionTable)
                if (row.CheckNodes(node1, node2))
                    return row;
            return null;
        }

        public void SendConnectionToNetworkNodeAgent(NodeConnectionInformation information) {
            SendConnectionToNetworkNodeAgent(information.NodeUdpPort,
                information.InVpi,
                information.InVci,
                information.InPortNumber,
                information.OutVpi,
                information.OutVci,
                information.OutPortNumber);
        }

        public void SendConnectionToNetworkNodeAgent(Connection connection) {
            foreach (var information in connection.Parameters.NodeConnectionInformations)
                SendConnectionToNetworkNodeAgent(information);
        }

        // Wpis w tablicy pola komutacyjnego
        public void SendConnectionToNetworkNodeAgent(int nodeUdpPort, int inVpi, int inVci, int inPortNumber, int outVpi,
            int outVci, int outPortNumber) {
            SendMessageToNetworkNode(
                "CreateConnection " + inVpi + " " + inVci + " " + inPortNumber + " " + outVpi + " " + outVci + " " +
                outPortNumber, nodeUdpPort);
            UpdateState(" Message to " + nodeUdpPort + ": " + "CreateConnection inVpi:" + inVpi + ", inVci: " + inVci +
                        ", inPort: " +
                        inPortNumber + ", outVpi: " + outVpi + ", outVci: " + outVci + ", outPort: " +
                        outPortNumber);
        }

        /* Wątek obsługujący keep alive*/

        private void RunThread() {
            while (true)
                if (_receivedMessagesList.Count > 0) {
                    var message = _receivedMessagesList[0].Split(' ');

                    switch (message[0]) {
                        case "networkNodeStart":
                            try {
                                _keepAliveDictionary.Add(int.Parse(message[1]), DateTime.Now);
                                UpdateState("Network node " + message[1] + " is online.");
                            }
                            catch (SystemException e) {
                            }
                            ;
                            //sendMessageToNetworkNode(Encoding.UTF8.GetBytes("OK " + int.Parse(message[1])), int.Parse(message[1]));
                            break;
                        case "keepAlive":
                            _keepAliveDictionary[int.Parse(message[1])] = DateTime.Now;
                            break;
                    }
                    _receivedMessagesList.Remove(_receivedMessagesList[0]);
                }
                else {
                    lock (_messageThread) {
                        Monitor.Wait(_messageThread);
                    }
                }
        }

        private void checkKeepAliveTable() {
            while (true) {
                try {
                    foreach (var node in _keepAliveDictionary)
                        if ((DateTime.Now - node.Value).TotalMilliseconds > MaxTimeNotReceivingKeepAliveMessage) {
                            _keepAliveDictionary.Remove(node.Key);
                            UpdateState("Network node " + node.Key + " is offline.");
                            Console.WriteLine("Network node " + node.Key + " is offline.");
                        }
                }
                catch (InvalidOperationException e) {
                }
                Thread.Sleep(500);
            }
        }

        public bool AreOnline(List<NetworkNodePictureBox> networkNodePictureBoxes) {
            var areOnline = true;

            foreach (var networkNodePictureBox in networkNodePictureBoxes) {
                var nodeUdpPort = networkNodePictureBox.Parameters.NetworkManagmentSystemDataPort;

                if (!IsOnline(nodeUdpPort)) {
                    areOnline = false;
                    break;
                }
            }

            return areOnline;
        }

        public bool IsOnline(int nodeUdpPort) {
            return _keepAliveDictionary.ContainsKey(nodeUdpPort);
        }

        private void AddToMessageList(string message) {
            _receivedMessagesList.Add(message);
            lock (_messageThread) {
                Monitor.Pulse(_messageThread);
            }
        }


        private void SendMessageToNetworkNode(string message, int port) {
            var bytes = Encoding.UTF8.GetBytes(message);
            var udpClient = new UdpClient();
            var ipEndpoint = new IPEndPoint(IPAddress.Loopback, port);
            udpClient.Send(bytes, bytes.Length, ipEndpoint);
        }

        private void ListenForConnectionRequests() {
            Task.Run(async () => {
                using (_listenUdpClient) {
                    while (true) {
                        //Console.WriteLine("czeka na zgloszenie");
                        var receivedData = await _listenUdpClient.ReceiveAsync();
                        //Console.WriteLine("Otrzymal " + System.Text.Encoding.UTF8.GetString(receivedData.Buffer));
                        var message = Encoding.UTF8.GetString(receivedData.Buffer);
                        AddToMessageList(message);
                        //EstabilishNodeConnection(BitConverter.ToInt32(receivedData.Buffer, 0));
                    }
                }
            });
        }

        public void Dispose() {
            _listenUdpClient.Close();
            UpdateState("Network Managment System shutting down.");
        }
    }
}