using System;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    public class NameServer : ConnectionManager {
        public static readonly NetworkAddress Address = new NetworkAddress("0");

        private readonly Directory _directory;
        private readonly Policy _policy;

        public NameServer(int listeningPort) : base(listeningPort) {
            _directory = new Directory(Address);
            _directory.UpdateState += (sender, state) => OnUpdateState(state);
            _directory.MessageToSend += OnMessageToSend;

            _policy = new Policy(Address);
            _policy.UpdateState += (sender, state) => OnUpdateState(state);
            _policy.MessageToSend += OnMessageToSend;
        }

        private void OnMessageToSend(object sender, SignallingMessage message) {
            Send(message, message.DestinationAddress);
            OnUpdateState("Sent " + message);
        }

        protected override void HandleReceivedObject(object receivedObject, NetworkAddress networkAddress) {
            var message = (SignallingMessage) receivedObject;
            Receive(message);
            OnUpdateState("Received " + message);
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