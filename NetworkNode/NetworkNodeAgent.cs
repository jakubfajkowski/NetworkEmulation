﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkNode {
    public class NetworkNodeAgent {
        private const int NmsPort = 6666;
        private const int SleepTimeKeepAlive = 500;
        private CommutationMatrix _commutationMatrix;

        private readonly CommutationTable _commutationTable;
        private IPEndPoint _ipEndpoint;

        private readonly UdpClient _listenUdpClient;
        public int ListenUdpPort;

        private UdpClient _udpClient;


        public NetworkNodeAgent(int port) {
            ListenUdpPort = port;

            _commutationTable = new CommutationTable();

            var ipEndPoint = new IPEndPoint(IPAddress.Any, ListenUdpPort);
            _listenUdpClient = new UdpClient(ipEndPoint);

            ListenForConnectionRequests();

            ConnectToNms();
        }

        private void ConnectToNms() {
            _udpClient = new UdpClient();
            _ipEndpoint = new IPEndPoint(IPAddress.Loopback, NmsPort);
            // Wiadomość, że węzeł wstał
            SendToNms(Encoding.UTF8.GetBytes("networkNodeStart " + ListenUdpPort));

            var keepAliveThread = new Thread(KeepAliveThreadRun);
            keepAliveThread.Start();
        }

        private void SendToNms(byte[] bytesToSend) {
            _udpClient.Send(bytesToSend, bytesToSend.Length, _ipEndpoint);
        }

        private void SendToNms(byte[] bytesToSend, int port) {
            _udpClient.Send(bytesToSend, bytesToSend.Length, new IPEndPoint(IPAddress.Loopback, port));
        }

        private void KeepAliveThreadRun() {
            var keepAliveMessage = Encoding.UTF8.GetBytes("keepAlive " + ListenUdpPort);
            while (true) {
                SendToNms(keepAliveMessage);
                Thread.Sleep(SleepTimeKeepAlive);
            }
        }

        private void ListenForConnectionRequests() {
            Task.Run(async () => {
                using (_listenUdpClient) {
                    while (true) {
                        //Console.WriteLine("czeka na zgloszenie od nms (w networkNode) "+ listenUdpPort);
                        var receivedData = await _listenUdpClient.ReceiveAsync();

                        var message = Encoding.UTF8.GetString(receivedData.Buffer);
                        Console.WriteLine("Odebral " + message);
                        var messageSplit = message.Split(' ');

                        switch (messageSplit[0]) {
                            case "CreateConnection":
                                AddConnectionToTable(int.Parse(messageSplit[1]), int.Parse(messageSplit[2]),
                                    int.Parse(messageSplit[3]),
                                    int.Parse(messageSplit[4]), int.Parse(messageSplit[5]), int.Parse(messageSplit[6]));
                                break;
                            case "CreatePortOut":
                                _commutationMatrix.CreateOutputPort(int.Parse(messageSplit[1]));
                                break;
                            case "CreatePortIn":
                                _commutationMatrix.CreateInputPort(int.Parse(messageSplit[1]));
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
            return _commutationTable.RemoveConnection(new CommutationTableRow(inVpi, inVci, inPortNumber, outVpi, outVci, outPortNumber));
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