using System;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.DataPlane;
using NetworkUtilities.Log;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.Network.Node {
    public abstract class Node : LogObject, IDisposable {
        private readonly ConnectionComponent _controlPlaneConnectionComponent;
        private readonly ConnectionComponent _dataPlaneConnectionComponent;
        protected int CableCloudListeningPort;
        protected int SignallingCloudListeningPort;


        protected Node(NetworkAddress networkAddress, string ipAddress, int cableCloudListeningPort,
            int signallingCloudListeningPort) {
            NetworkAddress = networkAddress;

            CableCloudListeningPort = cableCloudListeningPort;
            _dataPlaneConnectionComponent = new ConnectionComponent(NetworkAddress, ipAddress, 
                cableCloudListeningPort, ConnectionManagerType.CableCloud);
            _dataPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
            _dataPlaneConnectionComponent.ObjectReceived += OnCableCloudMessageReceived;

            SignallingCloudListeningPort = signallingCloudListeningPort;
            _controlPlaneConnectionComponent = new ConnectionComponent(NetworkAddress, ipAddress,
                signallingCloudListeningPort, ConnectionManagerType.SignallingCloud);
            _controlPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
            _controlPlaneConnectionComponent.ObjectReceived += OnSignallingMessageReceived;
        }

        public NetworkAddress NetworkAddress { get; }

        public int CableCloudDataPort { get; protected set; }

        public int PathComputationServerDataPort { get; protected set; }


        public bool Online => _dataPlaneConnectionComponent.Online && _controlPlaneConnectionComponent.Online;

        public virtual void Initialize() {
            _dataPlaneConnectionComponent.Initialize();
            _controlPlaneConnectionComponent.Initialize();
        }

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

        public void Dispose() {
            _dataPlaneConnectionComponent?.Dispose();
            _controlPlaneConnectionComponent?.Dispose();
        }
    }
}