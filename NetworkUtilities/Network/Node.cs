using System.Collections.Generic;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.Log;
using NetworkUtilities.Serialization;

namespace NetworkUtilities.Network {
    public abstract class Node : LogObject {
        private readonly ConnectionComponent _dataPlaneConnectionComponent;
        private readonly ConnectionComponent _controlPlaneConnectionComponent;
        protected int CableCloudListeningPort;
        protected int SignallingCloudListeningPort;


        protected Node(NetworkAddress networkAddress, string ipAddress, int cableCloudListeningPort,
            int signallingCloudListeningPort) {
            NetworkAddress = networkAddress;

            CableCloudListeningPort = cableCloudListeningPort;
            _dataPlaneConnectionComponent = new ConnectionComponent(NetworkAddress, ipAddress, cableCloudListeningPort);
            _dataPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
            _dataPlaneConnectionComponent.ObjectReceived += OnCableCloudMessageReceived;

            SignallingCloudListeningPort = signallingCloudListeningPort;
            _controlPlaneConnectionComponent = new ConnectionComponent(NetworkAddress, ipAddress,
                signallingCloudListeningPort);
            _controlPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
            _controlPlaneConnectionComponent.ObjectReceived += OnSignallingMessageReceived;
        }

        public virtual void Initialize() {
            _dataPlaneConnectionComponent.Initialize();
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