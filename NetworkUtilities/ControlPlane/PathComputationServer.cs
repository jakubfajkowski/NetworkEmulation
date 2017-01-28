using System;
using System.Collections.Generic;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    public abstract class PathComputationServer : ConnectionManager {
        private readonly NetworkAddress _networkAddress;
        private readonly Dictionary<NetworkAddress, int> _signallingLinkDictionary;

        public int PathComputationServerDataPort { get; protected set; }
        protected int PathComputationServerListeningPort;
        private readonly ConnectionComponent _controlPlaneConnectionComponent;

        protected PathComputationServer(NetworkAddress networkAddress, 
                                        string ipAddress, 
                                        int port, 
                                        int pathComputationServerListeningPort, 
                                        int pathComputationServerDataPort) : base(port) {

            PathComputationServerListeningPort = pathComputationServerListeningPort;
            PathComputationServerDataPort = pathComputationServerDataPort;
            _controlPlaneConnectionComponent = new ConnectionComponent(ipAddress, pathComputationServerListeningPort, pathComputationServerDataPort);
            _controlPlaneConnectionComponent.ObjectReceived += OnSignallingMessageReceived;

            _networkAddress = networkAddress;
            _signallingLinkDictionary = new Dictionary<NetworkAddress, int>();
        }

        public void Initialize() {
            _controlPlaneConnectionComponent.Initialize();
        }

        protected override void HandleReceivedObject(object receivedObject, int inputPort) {
            var signallingMessage = (SignallingMessage) receivedObject;

            OnUpdateState("Received from [" + signallingMessage.SourceAddress + "]: Element" + signallingMessage.DestinationControlPlaneElement +
                          "Operation" + signallingMessage.Operation + ".");

            if (signallingMessage.DestinationAddress.Equals(_networkAddress)) {
                Receive(signallingMessage);
            }
            else {
                int output = -1;

                try {
                    output = _signallingLinkDictionary[signallingMessage.DestinationAddress];

                    PassSignallingMessage(signallingMessage, output);
                }
                catch (KeyNotFoundException) {
                    OnUpdateState("Error sending to [" + signallingMessage.DestinationAddress + "]: There is no such record.");
                }
                catch (Exception) {
                    if (output != -1) DisconnectClient(output);
                    OnUpdateState("Error sending to [" + signallingMessage.DestinationAddress + "]: Could not connect.");
                }
            }
        }

        private void PassSignallingMessage(SignallingMessage signallingMessage, int outputPort) {
            SendObject(signallingMessage, outputPort);
            OnUpdateState("Sent to [" + signallingMessage.DestinationAddress + "]: Element" + signallingMessage.DestinationControlPlaneElement +
                          "Operation" + signallingMessage.Operation + ".");
        }

        public void AddSignallingLink(NetworkAddress networkAddress, int socketPort) {
            _signallingLinkDictionary.Add(networkAddress, socketPort);
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
