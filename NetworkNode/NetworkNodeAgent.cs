﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkNode {
    public class NetworkNodeAgent {
        private const int SleepTimeKeepAlive = 500;

        private readonly CommutationTable _commutationTable;

        private readonly UdpClient _listenUdpClient;
        private CommutationMatrix _commutationMatrix;
        private IPEndPoint _ipEndpoint;
        private NetworkNode _networkNode;
        private bool _timeToQuit;
        static Random random = new Random();
        private UdpClient _udpClient;
        public int ListenUdpPort;


        public NetworkNodeAgent(int port, NetworkNode networkNode) {
            ListenUdpPort = port;
            _networkNode = networkNode;
            _commutationTable = new CommutationTable();

            var ipEndPoint = new IPEndPoint(IPAddress.Any, ListenUdpPort);
            _listenUdpClient = new UdpClient(ipEndPoint);

            ListenForConnectionRequests();

            ConnectToNms();
        }

        public static int NmsPort { private get; set; } = 6666;

        private void ConnectToNms() {
            _udpClient = new UdpClient();
            _ipEndpoint = new IPEndPoint(IPAddress.Loopback, NmsPort);
        }

        public void StartThread() {
            // Wiadomość, że węzeł wstał
            SendToNms(Encoding.UTF8.GetBytes("networkNodeStart " + ListenUdpPort));
            _timeToQuit = false;
            var keepAliveThread = new Thread(KeepAliveThreadRun);
            keepAliveThread.Start();
        }

        public void Shutdown() {
            _timeToQuit = true;
        }

        private void SendToNms(byte[] bytesToSend) {
            _udpClient.Send(bytesToSend, bytesToSend.Length, _ipEndpoint);
        }

        private void SendToNms(byte[] bytesToSend, int port) {
            _udpClient.Send(bytesToSend, bytesToSend.Length, new IPEndPoint(IPAddress.Loopback, port));
        }

        private void KeepAliveThreadRun() {
            var keepAliveMessage = Encoding.UTF8.GetBytes("keepAlive " + ListenUdpPort);
            while (!_timeToQuit) {
                SendToNms(keepAliveMessage);
                Thread.Sleep(SleepTimeKeepAlive - 150 + random.Next(100));
            }
        }

        private void ListenForConnectionRequests() {
            Task.Run(async () => {
                using (_listenUdpClient) {
                    while (true) {
                        var receivedData = await _listenUdpClient.ReceiveAsync();

                        var message = Encoding.UTF8.GetString(receivedData.Buffer);
                        var messageSplit = message.Split(' ');

                        switch (messageSplit[0]) {
                            case "CreateConnection":
                                AddConnectionToTable(int.Parse(messageSplit[1]), int.Parse(messageSplit[2]),
                                    int.Parse(messageSplit[3]),
                                    int.Parse(messageSplit[4]), int.Parse(messageSplit[5]), int.Parse(messageSplit[6]));
                                break;
                        }
                    }
                }
            });
        }


        /* Wywołuje metodę tabeli połączeń, która dodaje połączenie */

        public void AddConnectionToTable(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci, int linkNumber) {
            _commutationTable.AddConnection(inVpi, inVci, inPortNumber, outVpi, outVci, linkNumber);
        }

        /* Wywołuje metodę tabeli połączeń, która usuwa połączenie */

        public bool RemoveConnectionFromTable(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci,
            int outPortNumber) {
            return
                _commutationTable.RemoveConnection(new CommutationTableRow(inVpi, inVci, inPortNumber, outVpi, outVci,
                    outPortNumber));
        }

        /* Getter potrzebny do tego, żeby przekazać obiekt do pola komutacyjnego (CommutationMatrix) */

        public CommutationTable GetCommutationTable() {
            return _commutationTable;
        }

        public void SetCommutationMatrix(CommutationMatrix cm) {
            _commutationMatrix = cm;
        }
    }
}