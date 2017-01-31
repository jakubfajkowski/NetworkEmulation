using System;
using NetworkUtilities.Log;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class NameServer : LogObject, IDisposable {
        public static readonly NetworkAddress Address = new NetworkAddress("0");

        private readonly ConnectionComponent _controlPlaneConnectionComponent;

        private Directory _directory;
        private Policy _policy;

        public NameServer(string ipAddress, int signallingCloudListeningPort) {
            _directory = new Directory(Address);
            Initialize(_directory);

            _policy = new Policy(Address);
            Initialize(_policy);

            _controlPlaneConnectionComponent = new ConnectionComponent(Address, ipAddress, 
                signallingCloudListeningPort, ConnectionManagerType.SignallingCloud);
            _controlPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
            _controlPlaneConnectionComponent.ObjectReceived += OnSignallingMessageReceived;
        }
        private void Initialize(ControlPlaneElement controlPlaneElement) {
            controlPlaneElement.UpdateState += (sender, state) => OnUpdateState(state);
            controlPlaneElement.MessageToSend +=
                (sender, message) => Send(message);
        }

        public void Initialize() {
            _controlPlaneConnectionComponent.Initialize();
        }

        public void UpdateDirectory(string clientName, SubnetworkPointPool snpp) {
            _directory.UpdateDirectory(clientName, snpp);
        }

        private void OnSignallingMessageReceived(object sender, object receivedObject) {
            var signallingMessage = (SignallingMessage) receivedObject;
            Receive(signallingMessage);
        }

        protected void Send(SignallingMessage signallingMessage) {
            _controlPlaneConnectionComponent.Send(signallingMessage);
        }

        private void Receive(SignallingMessage message) {
            switch (message.DestinationControlPlaneElement) {
                case ControlPlaneElementType.Directory:
                    _directory.ReceiveMessage(message);
                    break;

                case ControlPlaneElementType.Policy:
                    _policy.ReceiveMessage(message);
                    break;
            }
        }

        public void Dispose() {
            _controlPlaneConnectionComponent?.Dispose();

            _directory = new Directory(Address);
            Initialize(_directory);

            _policy = new Policy(Address);
            Initialize(_policy);
        }
    }
}