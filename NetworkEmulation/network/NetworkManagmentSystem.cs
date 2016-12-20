using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkEmulation.log;

namespace NetworkEmulation.network {
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

        public void SendConnectionToNetworkNodeAgent(NodeConnectionInformation info) {
            SendConnectionToNetworkNodeAgent(info.NodeUdpPort,
                                             info.InVpi,
                                             info.InVci,
                                             info.InPortNumber,
                                             info.OutVpi,
                                             info.OutVci,
                                             info.OutPortNumber);
        }

        // Wpis w tablicy pola komutacyjnego
        public void SendConnectionToNetworkNodeAgent(int nodeUdpPort, int inVpi, int inVci, int inPortNumber, int outVpi,
            int outVci, int outPortNumber) {
            SendMessageToNetworkNode(
                "CreateConnection " + inVpi + " " + inVci + " " + inPortNumber + " " + outVpi + " " + outVci + " " +
                outPortNumber, nodeUdpPort);
            UpdateState("Message to " + nodeUdpPort + ": " + "CreateConnection " + inVpi + " " + inVci + " " +
                        inPortNumber + " " + outVpi + " " + outVci + " " +
                        outPortNumber);
        }

        public void SendShutdownMessage(int nodeUdpPort) {
            SendMessageToNetworkNode("Shutdown", nodeUdpPort);
            UpdateState("Shutdown netowrk node " + nodeUdpPort);
        }

        // Wznowienie działania węzła
        public void SendStartMessage(int nodeUdpPort) {
            SendMessageToNetworkNode("Start", nodeUdpPort);
            UpdateState("Start netowrk node " + nodeUdpPort);
        }

        // Utworzenie portów do pola komutacyjnego 
        public void CreateLink(int node1, int node1OutPortNumber, int node2, int node2InPortNumber) {
            SendMessageToNetworkNode("CreatePortOut " + node1OutPortNumber, node1);
            SendMessageToNetworkNode("CreatePortIn " + node2InPortNumber, node2);
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
    }
}