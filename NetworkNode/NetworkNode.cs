using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NetworkUtilities;

namespace NetworkNode {
    public class NetworkNode : Node {
        // Czas po jakim komórki ATM zostaną spakowane w CCM
        public static int MinLastAddTime { private get; set; } = 4;
        // Czas usypiania wątku, który tworzy CCM
        public static int SleepTime { private get; set; } = 1;
        public static int NmsPort { private get; set; } = 6666;

        public CommutationMatrix CommutationMatrix;
        public NetworkNodeAgent NetworkNodeAgent;
        private readonly Thread _networkNodeThread;
        
        private bool _sent;

        private readonly bool _timeToQuit = false;

        public NetworkNode() {
            NetworkNodeAgent = new NetworkNodeAgent(RandomFreePort());
            CommutationMatrix = new CommutationMatrix(NetworkNodeAgent.GetCommutationTable());
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

                foreach (var port in CommutationMatrix.OutputPorts)
                    if (((port.GetAtmCellNumber() != 0) &&
                         ((DateTime.Now - port.GetLastAddTime()).TotalMilliseconds > MinLastAddTime)) ||
                        (port.GetAtmCellNumber() > CableCloudMessage.MaxAtmCellsNumber)) {
                        //Console.WriteLine("Rozpoczęcie tworzenia CableCloudMessage...");
                        var message = new CableCloudMessage(port.GetPortNumber());

                        var atmCellNumberInPort = port.GetAtmCellNumber();
                        //Console.WriteLine("Liczba komórek ATM w buforze wyjściowym: " + port.getATMCellNumber());
                        int atmCellNumberInMessage;
                        if (atmCellNumberInPort > CableCloudMessage.MaxAtmCellsNumber)
                            atmCellNumberInMessage = CableCloudMessage.MaxAtmCellsNumber;
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

        protected override void HandleMessage(CableCloudMessage cableCloudMessage) {
            ReceiveCableCloudMessage(cableCloudMessage);
        }

        public void ReceiveCableCloudMessage(CableCloudMessage cableCloudMessage) {
            Console.WriteLine("Message recieved on port: " + cableCloudMessage.PortNumber);

            foreach (var cell in cableCloudMessage.AtmCells)
                CommutationMatrix.AddAtmCellToInputPort(cell, cableCloudMessage.PortNumber);
        }

        private void SendCableCloudMessage(CableCloudMessage cableCloudMessage) {
            Send(cableCloudMessage.Serialize());
            Console.WriteLine("Message sent on port: " + cableCloudMessage.PortNumber);
        }
    }
}