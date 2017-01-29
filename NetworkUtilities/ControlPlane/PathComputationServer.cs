using System;
using System.Collections.Generic;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    public abstract class PathComputationServer : ConnectionManager {
        protected int PathComputationServerListeningPort;
        private readonly Dictionary<NetworkAddress, NetworkAddress> _signallingLinkDictionary;

        private readonly ConnectionComponent _controlPlaneConnectionComponent;

        protected PathComputationServer(NetworkAddress networkAddress, NetworkAddress otherPathComputationServerNetworkAddress, string ipAddress, int listeningPort, int pathComputationServerListeningPort) : base(listeningPort) {

            PathComputationServerListeningPort = pathComputationServerListeningPort;

            NetworkAddress = networkAddress;
            _signallingLinkDictionary = new Dictionary<NetworkAddress, NetworkAddress>();

            _controlPlaneConnectionComponent = new ConnectionComponent(networkAddress, otherPathComputationServerNetworkAddress, ipAddress,
                pathComputationServerListeningPort);
            _controlPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
            _controlPlaneConnectionComponent.ObjectReceived += OnSignallingMessageReceived;
        }

        public NetworkAddress NetworkAddress { get; }

        public virtual void Initialize() {
            _controlPlaneConnectionComponent.Initialize();
        }

        protected override void HandleReceivedObject(object receivedObject, NetworkAddress networkAddress) {
            var signallingMessage = (SignallingMessage) receivedObject;

            OnUpdateState("Received from [" + signallingMessage.SourceAddress + "]: Element" +
                          signallingMessage.DestinationControlPlaneElement +
                          "Operation" + signallingMessage.Operation + ".");

            if (signallingMessage.DestinationAddress.Equals(NetworkAddress)) {
                Receive(signallingMessage);
            }
            else {
                NetworkAddress output = null;

                try {
                    output = _signallingLinkDictionary[signallingMessage.DestinationAddress];

                    SendSignallingMessage(signallingMessage, output);
                }
                catch (KeyNotFoundException) {
                    OnUpdateState("Error sending to [" + signallingMessage.DestinationAddress +
                                  "]: There is no such record.");
                }
                catch (Exception) {
                    if (output != null) DeleteConnection(output);
                    OnUpdateState("Error sending to [" + signallingMessage.DestinationAddress + "]: Could not connect.");
                }
            }
        }

        protected void SendSignallingMessage(SignallingMessage signallingMessage, NetworkAddress outputNetworkAddress) {
            Send(signallingMessage, outputNetworkAddress);
            OnUpdateState("Sent to [" + signallingMessage.DestinationAddress + "]: Element" +
                          signallingMessage.DestinationControlPlaneElement +
                          "Operation" + signallingMessage.Operation + ".");
        }

        public void AddSignallingLink(NetworkAddress inputNetworkAddress, NetworkAddress outputNetworkAddress) {
            _signallingLinkDictionary.Add(inputNetworkAddress, outputNetworkAddress);
        }

        protected void OnSignallingMessageReceived(object sender, object receivedObject) {
            var signallingMessage = (SignallingMessage) receivedObject;
            Receive(signallingMessage);
        }

        protected abstract void Receive(SignallingMessage signallingMessage);

        protected void SendToOtherPathComputationServer(SignallingMessage signallingMessage) {
            _controlPlaneConnectionComponent.Send(signallingMessage);
        }
    }
}