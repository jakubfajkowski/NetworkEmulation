using System;
using System.Threading;
using NetworkUtilities;
using NetworkUtilities.element;

namespace NetworkNode {
    public class NetworkNode : Node {
        // Czas po jakim komórki ATM zostaną spakowane w CCM
        public static int MinLastAddTime { private get; set; } = 4;

        public CommutationMatrix CommutationMatrix;
        public NetworkNodeAgent NetworkNodeAgent;
        private Thread _networkNodeThread;

        private bool _timeToQuit;

        public NetworkNode(NetworkNodeSerializableParameters parameters) : base(parameters.IpAddress, parameters.CableCloudListeningPort, parameters.CableCloudDataPort) {
            
            NetworkNodeAgent.NmsPort = parameters.NetworkManagmentSystemListeningPort;
            NetworkNodeAgent = new NetworkNodeAgent(parameters.NetworkManagmentSystemDataPort, this);
            Console.WriteLine("nr portu " + parameters.NetworkManagmentSystemDataPort);
            CommutationMatrix = new CommutationMatrix(NetworkNodeAgent.GetCommutationTable(), parameters.NumberOfPorts);
            NetworkNodeAgent.SetCommutationMatrix(CommutationMatrix);

            startThread();     
        }

        public void startThread()
        {
            _timeToQuit = false;
            _networkNodeThread = new Thread(RunThread);
            _networkNodeThread.Start();
            CommutationMatrix.startThread();
            NetworkNodeAgent.startThread();
        }

        public void shutdown()
        {
            _timeToQuit = true;
            CommutationMatrix.shutdown();
            NetworkNodeAgent.shutdown();
        }

        /* Wątek pobierający komórki ATM z portów wyjściowych pola komutacyjnego i wysyłający je do chmury kablowej */
        private void RunThread() {
            var j = 0;
            while (!_timeToQuit) {
                
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

                       // Console.WriteLine(DateTime.Now.Millisecond + "  Wysyłanie CableCloudMessage na port " +
                       //                   message.PortNumber + " Liczba ATMCell: " + message.AtmCells.Count
                       //                   + " Port: " + port.GetPortNumber());
                        SendCableCloudMessage(message);
                       
                    }
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
            cableCloudMessage.Fill();
            Send(cableCloudMessage.Serialize());
            Console.WriteLine("Message sent on port: " + cableCloudMessage.PortNumber);
        }
    }
}