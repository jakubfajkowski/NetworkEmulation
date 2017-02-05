using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class SignallingCloud : ConnectionManager {
        public SignallingCloud(int listeningPort) : 
            base(listeningPort, ConnectionManagerType.SignallingCloud) {}

        protected override void HandleReceivedObject(object receivedObject, NetworkAddress inputNetworkAddress) {
            var signallingMessage = (SignallingMessage) receivedObject;
            OnUpdateState(signallingMessage.ToString());
            OnUpdateState("SESSION: " + signallingMessage.SessionId);
            Send(signallingMessage, signallingMessage.DestinationAddress);
        }
    }
}