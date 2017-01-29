using System;
using System.Collections.Generic;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    public abstract class PathComputationServer : ConnectionManager {
        protected int PathComputationServerListeningPort;

        private readonly ConnectionComponent _controlPlaneConnectionComponent;

        protected PathComputationServer(NetworkAddress networkAddress, NetworkAddress otherPathComputationServerNetworkAddress, string ipAddress, int listeningPort, int pathComputationServerListeningPort) : base(listeningPort) {

            PathComputationServerListeningPort = pathComputationServerListeningPort;

            NetworkAddress = networkAddress;

            _controlPlaneConnectionComponent = new ConnectionComponent(networkAddress, otherPathComputationServerNetworkAddress, ipAddress,
                pathComputationServerListeningPort);
            _controlPlaneConnectionComponent.ConnectionEstablished += ControlPlaneConnectionComponentOnConnectionEstablished;
            _controlPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
            _controlPlaneConnectionComponent.ObjectReceived += OnSignallingMessageReceived;
        }

        protected void OnSignallingMessageReceived(object sender, object receivedObject) {
            var signallingMessage = (SignallingMessage) receivedObject;

            HandleReceivedObject(signallingMessage, null);
        }

        private void ControlPlaneConnectionComponentOnConnectionEstablished(object sender, ConnectionHandlerArgs args) {
            AddConnection(args.NetworkAddress, args.TcpClient);
        }

        public NetworkAddress NetworkAddress { get; }

        public virtual void Initialize() {
            _controlPlaneConnectionComponent.Initialize();
        }

        protected override void HandleReceivedObject(object receivedObject, NetworkAddress networkAddress) {
            var signallingMessage = (SignallingMessage) receivedObject;

            OnUpdateState("Received " + signallingMessage);

            if (signallingMessage.DestinationAddress.Equals(NetworkAddress)) {
                Receive(signallingMessage);
            }
            else {
                try {
                    SendSignallingMessage(signallingMessage, signallingMessage.DestinationAddress);
                }
                catch (KeyNotFoundException) {
                    OnUpdateState("Error sending " + signallingMessage  + ": There is no such record.");
                }
                catch (Exception) {
                    OnUpdateState("Error sending " + signallingMessage + ": Could not connect.");
                }
            }
        }

        protected void SendSignallingMessage(SignallingMessage signallingMessage, NetworkAddress outputNetworkAddress) {
            Send(signallingMessage, outputNetworkAddress);
            OnUpdateState("Sent " + signallingMessage);
        }

        protected abstract void Receive(SignallingMessage signallingMessage);

        protected void SendToOtherPathComputationServer(SignallingMessage signallingMessage) {
            _controlPlaneConnectionComponent.Send(signallingMessage);
        }
    }
}