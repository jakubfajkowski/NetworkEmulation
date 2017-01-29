using System.Collections.Generic;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.Log;
using NetworkUtilities.Serialization;

namespace NetworkUtilities.Network {
    public abstract class Node : LogObject {
        private readonly ConnectionComponent _controlPlaneConnectionComponent;
        private readonly ConnectionComponent _dataPlaneConnectionComponent;
        protected int CableCloudListeningPort;
        protected int PathComputationServerListeningPort;


        protected Node(NetworkAddress networkAddress, NetworkAddress pathcomputationServerNetworkAddress, string ipAddress, int cableCloudListeningPort,
            int pathComputationServerListeningPort) {
            NetworkAddress = networkAddress;

            CableCloudListeningPort = cableCloudListeningPort;
            _dataPlaneConnectionComponent = new ConnectionComponent(NetworkAddress, null, ipAddress, cableCloudListeningPort);
            _dataPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
            _dataPlaneConnectionComponent.ObjectReceived += OnCableCloudMessageReceived;
            _dataPlaneConnectionComponent.Initialize();

            PathComputationServerListeningPort = pathComputationServerListeningPort;
            _controlPlaneConnectionComponent = new ConnectionComponent(NetworkAddress, pathcomputationServerNetworkAddress, ipAddress,
                pathComputationServerListeningPort);
            _controlPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
            _controlPlaneConnectionComponent.ObjectReceived += OnSignallingMessageReceived;
            _controlPlaneConnectionComponent.Initialize();
        }

        public NetworkAddress NetworkAddress { get; }

        public int CableCloudDataPort { get; protected set; }

        public int PathComputationServerDataPort { get; protected set; }


        public bool Online => _dataPlaneConnectionComponent.Online && _controlPlaneConnectionComponent.Online;

        private void OnCableCloudMessageReceived(object sender, object receivedObject) {
            var cableCloudMessage = (CableCloudMessage) receivedObject;
            Receive(cableCloudMessage);
        }

        protected abstract void Receive(CableCloudMessage cableCloudMessage);

        protected void Send(CableCloudMessage cableCloudMessage) {
            _dataPlaneConnectionComponent.Send(cableCloudMessage);
        }

        private void OnSignallingMessageReceived(object sender, object receivedObject) {
            var signallingMessage = (SignallingMessage) receivedObject;
            Receive(signallingMessage);
        }

        protected abstract void Receive(SignallingMessage signallingMessage);

        protected void Send(SignallingMessage signallingMessage) {
            _controlPlaneConnectionComponent.Send(signallingMessage);
        }
    }
}