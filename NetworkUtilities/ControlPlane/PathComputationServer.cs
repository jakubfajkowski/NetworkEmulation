using System.Collections.Generic;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    class PathComputationServer : ConnectionManager {
        private NetworkAddress _networkAddress;
        private readonly Dictionary<NetworkAddress, int> _signallingLinkDictionary;

        public PathComputationServer(NetworkAddress networkAddress, int port) : base(port) {
            _networkAddress = networkAddress;
            _signallingLinkDictionary = new Dictionary<NetworkAddress, int>();
        }

        protected override void HandleReceivedObject(object receivedObject, int inputPort) {
            var signallingMessage = (SignallingMessage) receivedObject;

        }
    }
}
