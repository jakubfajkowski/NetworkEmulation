using System;
using NetworkUtilities.Log;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public abstract class PathComputationServer : LogObject, IDisposable {
        private readonly ConnectionComponent _controlPlaneConnectionComponent;
        protected int SignallingCloudListeningPort;

        protected PathComputationServer(NetworkAddress networkAddress, string ipAddress,
            int signallingCloudListeningPort) {
            SignallingCloudListeningPort = signallingCloudListeningPort;

            NetworkAddress = networkAddress;

            _controlPlaneConnectionComponent = new ConnectionComponent(networkAddress, ipAddress,
                signallingCloudListeningPort, ConnectionManagerType.SignallingCloud);
            _controlPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
            _controlPlaneConnectionComponent.ObjectReceived += OnSignallingMessageReceived;
        }

        protected void Initialize(ControlPlaneElement controlPlaneElement) {
            controlPlaneElement.UpdateState += (sender, state) => OnUpdateState(state);
            controlPlaneElement.MessageToSend +=
                (sender, message) => Send(message);
        }

        public NetworkAddress NetworkAddress { get; }

        protected void OnSignallingMessageReceived(object sender, object receivedObject) {
            var signallingMessage = (SignallingMessage) receivedObject;
            Receive(signallingMessage);
        }

        public virtual void Initialize() {
            _controlPlaneConnectionComponent.Initialize();
        }

        protected void Send(SignallingMessage signallingMessage) {
            _controlPlaneConnectionComponent.Send(signallingMessage);
        }

        protected abstract void Receive(SignallingMessage signallingMessage);

        protected void SendToOtherPathComputationServer(SignallingMessage signallingMessage) {
            _controlPlaneConnectionComponent.Send(signallingMessage);
        }

        public virtual void Dispose() {
            _controlPlaneConnectionComponent?.Dispose();
        }
    }
}