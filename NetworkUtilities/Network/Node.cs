using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkUtilities.Log;
using NetworkUtilities.Serialization;

namespace NetworkUtilities.Network {
    public abstract class Node : LogObject {
        private readonly ConnectionComponent _dataPlaneConnectionComponent;
        private ConnectionComponent _controlPlaneConnectionComponent;


        public int CableCloudDataPort { get; protected set; }
        public bool Online => _dataPlaneConnectionComponent.Online;

        protected int CableCloudListeningPort;
        protected NetworkAddress NodeNetworkAddress;

        protected Node(string ipAddress, int cableCloudListeningPort, int cableCloudDataPort) {
            _dataPlaneConnectionComponent = new ConnectionComponent(ipAddress, cableCloudListeningPort, cableCloudDataPort);
            _dataPlaneConnectionComponent.ObjectReceived += OnCableCloudMessageReceived;

            CableCloudListeningPort = cableCloudListeningPort;
            CableCloudDataPort = cableCloudDataPort;
        }

        private void OnCableCloudMessageReceived(object sender, object receivedObject) {
            var cableCloudMessage = (CableCloudMessage) receivedObject;
            Recieve(cableCloudMessage);
        }

        protected abstract void Recieve(CableCloudMessage cableCloudMessage);

        protected void Send(CableCloudMessage cableCloudMessage) {
            _dataPlaneConnectionComponent.SendObject(cableCloudMessage);
        }

        public List<AtmCell> ExtractAtmCells(CableCloudMessage cableCloudMessage) {
            var atmCells = BinarySerializer.Deserialize(cableCloudMessage.Data) as List<AtmCell>;
            return atmCells?.FindAll(cell => cell.Valid());
        }

    }
}