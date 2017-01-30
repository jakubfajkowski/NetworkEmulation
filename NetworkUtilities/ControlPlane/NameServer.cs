using System;
using NetworkUtilities.Log;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    public class NameServer : LogObject {
        public static readonly NetworkAddress Address = new NetworkAddress("0");

        private readonly ConnectionComponent _controlPlaneConnectionComponent;

        private readonly Directory _directory;
        private readonly Policy _policy;

        public NameServer(string ipAddress, int signallingCloudListeningPort) {
            _directory = new Directory(Address);
            _directory.UpdateState += (sender, state) => OnUpdateState(state);
            _directory.MessageToSend += OnMessageToSend;

            _policy = new Policy(Address);
            _policy.UpdateState += (sender, state) => OnUpdateState(state);
            _policy.MessageToSend += OnMessageToSend;

            _controlPlaneConnectionComponent = new ConnectionComponent(Address, ipAddress, signallingCloudListeningPort);
            _controlPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
            _controlPlaneConnectionComponent.ObjectReceived += OnSignallingMessageReceived;
        }

        public void Initialize() {
            _controlPlaneConnectionComponent.Initialize();
        }

        public void UpdateDirectory(string clientName, SubnetworkPointPool snpp) {
            _directory.UpdateDirectory(clientName, snpp);
        } 

        private void OnMessageToSend(object sender, SignallingMessage message) {
            _controlPlaneConnectionComponent.Send(message);
        }

        private void OnSignallingMessageReceived(object sender, object receivedObject) {
            var signallingMessage = (SignallingMessage)receivedObject;
            Receive(signallingMessage);
        }

        private void Receive(SignallingMessage message) {
            switch (message.DestinationControlPlaneElement) {
                case SignallingMessageDestinationControlPlaneElement.Directory:
                    _directory.ReceiveMessage(message);
                    break;

                case SignallingMessageDestinationControlPlaneElement.Policy:
                    _policy.ReceiveMessage(message);
                    break;
            }
        }
    }
}