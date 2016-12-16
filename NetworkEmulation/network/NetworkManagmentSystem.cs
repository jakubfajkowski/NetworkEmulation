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
        private readonly List<ConnectionTableRow> _connectionTable;
        private readonly Dictionary<int, DateTime> _keepAliveDictionary;
        private readonly UdpClient _listenUdpClient;
        private readonly Thread _messageThread;
        private List<string> _receivedConnectionLabelsList;

        private readonly List<string> _receivedMessagesList;


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
        }


        public ConnectionTableRow GetRow(int node1, int node2) {
            foreach (var row in _connectionTable)
                if (row.CheckNodes(node1, node2))
                    return row;
            return null;
        }

        // Wpis w tablicy pola komutacyjnego
        public void SendConnectionToNetworkNodeAgent(int nodeUdpPort, int inVpi, int inVci, int inPortNumber, int outVpi,
            int outVci, int outPortNumber) {
            SendMessageToNetworkNode(
                "CreateConnection " + inVpi + " " + inVci + " " + inPortNumber + " " + outVpi + " " + outVci + " " +
                outPortNumber, nodeUdpPort);
            UpdateState("Message to " + nodeUdpPort + ": " +  "CreateConnection " + inVpi + " " + inVci + " " + inPortNumber + " " + outVpi + " " + outVci + " " +
                outPortNumber);
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
                            _keepAliveDictionary.Add(int.Parse(message[1]), DateTime.Now);
                            //sendMessageToNetworkNode(Encoding.UTF8.GetBytes("OK " + int.Parse(message[1])), int.Parse(message[1]));
                            break;
                        case "keepAlive":
                            //Console.WriteLine(receivedMessagesList[0]);
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