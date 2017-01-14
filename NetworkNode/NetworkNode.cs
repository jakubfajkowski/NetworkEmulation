using System;
using System.Collections.Generic;
using System.Threading;
using NetworkUtilities;
using NetworkUtilities.Element;

namespace NetworkNode {
    public class NetworkNode : Node {
        
        // Czas po jakim komórki ATM zostaną spakowane w CCM
        public static int MinLastAddTime { private get; set; } = 100;
       
        public CommutationMatrix CommutationMatrix;
        public NetworkNodeAgent NetworkNodeAgent;
        private LinkResourceManager linkResourceManager;


        public NetworkNode(NetworkNodeModel parameters)
            : base(parameters.IpAddress, parameters.CableCloudListeningPort, parameters.CableCloudDataPort)
        {
            CableCloudMessage.MaxAtmCellsNumber = parameters.MaxAtmCellsNumberInCableCloudMessage;
            NetworkNodeAgent.NmsPort = parameters.NetworkManagmentSystemListeningPort;
            NetworkNodeAgent = new NetworkNodeAgent(parameters.NetworkManagmentSystemDataPort, this);

            Console.WriteLine("Network Node \nNMS port: " + parameters.NetworkManagmentSystemDataPort +
                              "\nCableCloud port: " + parameters.CableCloudDataPort);

            CommutationMatrix = new CommutationMatrix(NetworkNodeAgent.GetCommutationTable(), parameters.NumberOfPorts);
            NetworkNodeAgent.SetCommutationMatrix(CommutationMatrix);

            linkResourceManager = new LinkResourceManager(NetworkNodeAgent.GetCommutationTable(), parameters.NumberOfPorts, 300); // ZMIENIC CAPACITY
        }


        protected override void Recieve(CableCloudMessage cableCloudMessage) {
            ReceiveCableCloudMessage(cableCloudMessage);        
        }

        public void ReceiveCableCloudMessage(CableCloudMessage cableCloudMessage) {
            Console.WriteLine("[" + DateTime.Now + "] Message recieved on port: " + cableCloudMessage.PortNumber);
            Console.WriteLine("[" + DateTime.Now + "] Received " + AtmCells(cableCloudMessage).Count + " atmcells");

            /* foreach (var cell in AtmCells(cableCloudMessage))
                 CommutationMatrix.AddAtmCellToInputPort(cell, cableCloudMessage.PortNumber);
                 */
                
            SendCableCloudMessage(CommutationMatrix.CommuteAllCells(AtmCells(cableCloudMessage),cableCloudMessage.PortNumber));
        }

        private void SendCableCloudMessage(CableCloudMessage cableCloudMessage) {
            Send(cableCloudMessage);
            Console.WriteLine("[" + DateTime.Now + "] Message sent on port: " + cableCloudMessage.PortNumber);
            Console.WriteLine("[" + DateTime.Now + "] Sent " + AtmCells(cableCloudMessage).Count + " atmcells");
        }
    }
}