using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.Log;
using NetworkUtilities.Serialization;

namespace NetworkUtilities.Network {
    public abstract class Node : LogObject {
        protected NetworkAddress NodeNetworkAddress;

        public int CableCloudDataPort { get; protected set; }
        protected int CableCloudListeningPort;
        private readonly ConnectionComponent _dataPlaneConnectionComponent;

        public int PathComputationServerDataPort { get; protected set; }
        protected int PathComputationServerListeningPort;
        private readonly ConnectionComponent _controlPlaneConnectionComponent;



        public bool Online => _dataPlaneConnectionComponent.Online && _controlPlaneConnectionComponent.Online;
   

        protected Node(string ipAddress, int cableCloudListeningPort, int cableCloudDataPort, 
                                         int pathComputationServerListeningPort, int pathComputationServerDataPort) {
            CableCloudListeningPort = cableCloudListeningPort;
            CableCloudDataPort = cableCloudDataPort;
            _dataPlaneConnectionComponent = new ConnectionComponent(ipAddress, cableCloudListeningPort, cableCloudDataPort);
            _dataPlaneConnectionComponent.ObjectReceived += OnCableCloudMessageReceived;

            PathComputationServerListeningPort = pathComputationServerListeningPort;
            PathComputationServerDataPort = pathComputationServerDataPort;
            _controlPlaneConnectionComponent = new ConnectionComponent(ipAddress, pathComputationServerListeningPort, pathComputationServerDataPort);
            _controlPlaneConnectionComponent.ObjectReceived += OnSignallingMessageReceived;
        }

        private void OnCableCloudMessageReceived(object sender, object receivedObject) {
            var cableCloudMessage = (CableCloudMessage) receivedObject;
            Receive(cableCloudMessage);
        }

        protected abstract void Receive(CableCloudMessage cableCloudMessage);

        protected void Send(CableCloudMessage cableCloudMessage) {
            _dataPlaneConnectionComponent.SendObject(cableCloudMessage);
        }

        public List<AtmCell> ExtractAtmCells(CableCloudMessage cableCloudMessage) {
            var atmCells = BinarySerializer.Deserialize(cableCloudMessage.Data) as List<AtmCell>;
            return atmCells?.FindAll(cell => cell.Valid());
        }

        private void OnSignallingMessageReceived(object sender, object receivedObject) {
            var signallingMessage = (SignallingMessage)receivedObject;
            Receive(signallingMessage);
        }

        protected abstract void Receive(SignallingMessage signallingMessage);

        protected void Send(SignallingMessage signallingMessage) {
            _dataPlaneConnectionComponent.SendObject(signallingMessage);
        }
    }
}