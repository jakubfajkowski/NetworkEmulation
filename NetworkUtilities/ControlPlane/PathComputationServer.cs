using System;
using System.Collections.Generic;
using System.Threading;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    public abstract class PathComputationServer : ConnectionManager {
        public NetworkAddress NetworkAddress { get; }
        private readonly Dictionary<NetworkAddress, NetworkAddress> _signallingLinkDictionary;

        public int OutputPort { get; protected set; }
        protected int PathComputationServerListeningPort;
        private readonly ConnectionComponent _controlPlaneConnectionComponent;

        protected PathComputationServer(NetworkAddress networkAddress, 
                                        string ipAddress, 
                                        int listeningPort, 
                                        int pathComputationServerListeningPort, 
                                        int pathComputationServerDataPort) : base(listeningPort) {

            PathComputationServerListeningPort = pathComputationServerListeningPort;
            OutputPort = pathComputationServerDataPort;
            _controlPlaneConnectionComponent = new ConnectionComponent(networkAddress, OutputPort, ipAddress, pathComputationServerListeningPort);
            _controlPlaneConnectionComponent.ObjectReceived += OnSignallingMessageReceived;

            NetworkAddress = networkAddress;
            _signallingLinkDictionary = new Dictionary<NetworkAddress, NetworkAddress>();
        }

        public void Initialize() {
            _controlPlaneConnectionComponent.Initialize();
        }

        protected override void HandleReceivedObject(object receivedObject, NetworkAddress networkAddress) {
            var signallingMessage = (SignallingMessage) receivedObject;

            OnUpdateState("Received from [" + signallingMessage.SourceAddress + "]: Element" + signallingMessage.DestinationControlPlaneElement +
                          "Operation" + signallingMessage.Operation + ".");

            if (signallingMessage.DestinationAddress.Equals(NetworkAddress)) {
                Receive(signallingMessage);
            }
            else {
                NetworkAddress output = null;

                try {
                    output = _signallingLinkDictionary[signallingMessage.DestinationAddress];

                    PassSignallingMessage(signallingMessage, output);
                }
                catch (KeyNotFoundException) {
                    OnUpdateState("Error sending to [" + signallingMessage.DestinationAddress + "]: There is no such record.");
                }
                catch (Exception) {
                    if (output != null) DisconnectClient(output);
                    OnUpdateState("Error sending to [" + signallingMessage.DestinationAddress + "]: Could not connect.");
                }
            }
        }

        private void PassSignallingMessage(SignallingMessage signallingMessage, NetworkAddress outputNetworkAddress) {
            SendObject(signallingMessage, outputNetworkAddress);
            OnUpdateState("Sent to [" + signallingMessage.DestinationAddress + "]: Element" + signallingMessage.DestinationControlPlaneElement +
                          "Operation" + signallingMessage.Operation + ".");
        }

        public void AddSignallingLink(NetworkAddress inputNetworkAddress, NetworkAddress outputNetworkAddress) {
            _signallingLinkDictionary.Add(inputNetworkAddress, outputNetworkAddress);
        }

        private void OnSignallingMessageReceived(object sender, object receivedObject) {
            var signallingMessage = (SignallingMessage)receivedObject;
            Receive(signallingMessage);
        }

        protected abstract void Receive(SignallingMessage signallingMessage);

        protected void Send(SignallingMessage signallingMessage) {
            _controlPlaneConnectionComponent.SendObject(signallingMessage);
        }
    }
}
