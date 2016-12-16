using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkUtilities;

namespace NetworkNode {
    public class NetworkNode : Node {
        // Czas po jakim komórki ATM zostaną spakowane w CCM
        private const int MinLastAddTime = 4;
        private const int MaxAtmCellNumberInCableCloudMessage = 9;
        // Czas usypiania wątku, który tworzy CCM
        private const int SleepTime = 1;
        // private const int nmsPort = 6666;

        public int AgentPort;
        private readonly TcpListener _agentTcpListener;

        public CommutationMatrix CommutationMatrix;
        public NetworkNodeAgent NetworkNodeAgent;
        private readonly Thread _networkNodeThread;

        private readonly List<Port> _outputCommutationMatrixPorts;
        private bool _sent;

        private readonly bool _timeToQuit = false;

        //  private UdpClient udpClient;
        //  private IPEndPoint ipEndpoint;
        //  private const int sleepTimeKeepAlive = 500;


        public NetworkNode() {
            AgentPort = FreeTcpPort();
            _agentTcpListener = CreateTcpListener(IPAddress.Loopback, AgentPort);
            ListenForConnectRequest(_agentTcpListener);

            NetworkNodeAgent = new NetworkNodeAgent(FreeTcpPort());
            CommutationMatrix = new CommutationMatrix(NetworkNodeAgent.GetCommutationTable(), this);
            _outputCommutationMatrixPorts = CommutationMatrix.GetOutputPortList();
            NetworkNodeAgent.SetCommutationMatrix(CommutationMatrix);

            _networkNodeThread = new Thread(RunThread);
            _networkNodeThread.Start();
        }

        /* Wątek pobierający komórki ATM z portów wyjściowych pola komutacyjnego i wysyłający je do chmury kablowej */

        private void RunThread() {
            var j = 0;
            while (!_timeToQuit) {
                _sent = false;
                if (j < 0)
                    Console.WriteLine(DateTime.Now.Millisecond + "  Wywolanie run outBuffer: " + j++);

                foreach (var port in _outputCommutationMatrixPorts)
                    if (((port.GetAtmCellNumber() != 0) &&
                         ((DateTime.Now - port.GetLastAddTime()).TotalMilliseconds > MinLastAddTime)) ||
                        (port.GetAtmCellNumber() > MaxAtmCellNumberInCableCloudMessage)) {
                        //Console.WriteLine("Rozpoczęcie tworzenia CableCloudMessage...");
                        var message = new CableCloudMessage(port.GetPortNumber());

                        var atmCellNumberInPort = port.GetAtmCellNumber();
                        //Console.WriteLine("Liczba komórek ATM w buforze wyjściowym: " + port.getATMCellNumber());
                        int atmCellNumberInMessage;
                        if (atmCellNumberInPort > MaxAtmCellNumberInCableCloudMessage)
                            atmCellNumberInMessage = MaxAtmCellNumberInCableCloudMessage;
                        else
                            atmCellNumberInMessage = atmCellNumberInPort;

                        for (var i = 0; i < atmCellNumberInMessage; i++)
                            message.Add(port.GetAtmCell());

                        Console.WriteLine(DateTime.Now.Millisecond + "  Wysyłanie CableCloudMessage na port " +
                                          message.PortNumber + " Liczba ATMCell: " + message.AtmCells.Count
                                          + " Port: " + port.GetPortNumber());
                        SendCableCloudMessage(message);
                        _sent = true;
                    }

                if (!_sent)
                    Thread.Sleep(SleepTime);
            }
        }

        public void ReceiveCableCloudMessage(CableCloudMessage message) {
            foreach (var cell in message.AtmCells)
                CommutationMatrix.AddAtmCellToInputPort(cell, message.PortNumber);
        }

        private void SendCableCloudMessage(CableCloudMessage message) {
            Send(CableCloudMessage.Serialize(message));
        }

        /* Metoda wywoływana po wczytaniu danych z wejścia */

        protected override void HandleMessage(CableCloudMessage cableCloudMessage) {
            Console.WriteLine("NetworkNode - HandleMessage: " + cableCloudMessage.PortNumber);
            ReceiveCableCloudMessage(cableCloudMessage);
        }
    }
}