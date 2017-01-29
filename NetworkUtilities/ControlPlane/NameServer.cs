using System;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    public class NameServer : ConnectionManager {
        public static readonly NetworkAddress Address = new NetworkAddress("0");

        private readonly Directory _directory;
        private readonly Policy _policy;

        public NameServer(int listeningPort) : base(listeningPort) {
            _directory = new Directory(Address);
            _directory.MessageToSend += MessageToSend;
            _policy = new Policy(Address);
            _policy.MessageToSend += MessageToSend;
        }

        private void MessageToSend(object sender, SignallingMessage message) {
            SendObject(message, message.DestinationAddress);
        }

        protected override void HandleReceivedObject(object receivedObject, NetworkAddress networkAddress) {
            Receive((SignallingMessage) receivedObject);
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