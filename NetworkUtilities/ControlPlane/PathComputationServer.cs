using System;
using System.Collections.Generic;
using NetworkUtilities.Log;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    public abstract class PathComputationServer : LogObject {
        protected int SignallingCloudListeningPort;

        private readonly ConnectionComponent _controlPlaneConnectionComponent;

        protected PathComputationServer(NetworkAddress networkAddress, string ipAddress, int signallingCloudListeningPort) {
            SignallingCloudListeningPort = signallingCloudListeningPort;

            NetworkAddress = networkAddress;

            _controlPlaneConnectionComponent = new ConnectionComponent(networkAddress, ipAddress, signallingCloudListeningPort);
            _controlPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
            _controlPlaneConnectionComponent.ObjectReceived += OnSignallingMessageReceived;
        }

        protected void OnSignallingMessageReceived(object sender, object receivedObject) {
            var signallingMessage = (SignallingMessage) receivedObject;
            Receive(signallingMessage);
        }

        public NetworkAddress NetworkAddress { get; }

        public virtual void Initialize() {
            _controlPlaneConnectionComponent.Initialize();
        }

        protected void SendSignallingMessage(SignallingMessage signallingMessage, NetworkAddress outputNetworkAddress) {
            _controlPlaneConnectionComponent.Send(signallingMessage);
        }

        protected abstract void Receive(SignallingMessage signallingMessage);

        protected void SendToOtherPathComputationServer(SignallingMessage signallingMessage) {
            _controlPlaneConnectionComponent.Send(signallingMessage);
        }
    }
}