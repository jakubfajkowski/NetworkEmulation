using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    class PathComputationServer : ConnectionManager {

        public PathComputationServer(int port) : base(port) {}
        protected override void HandleReceivedObject(object receivedObject, int inputPort) {
            var signallingMessage = (SignallingMessage) receivedObject;

            
        }
    }
}
