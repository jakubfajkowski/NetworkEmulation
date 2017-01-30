using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities.Network;

namespace NetworkUtilities.ControlPlane {
    public class SignallingCloud : ConnectionManager {
        public SignallingCloud(int listeningPort) : base(listeningPort) {}

        protected override void HandleReceivedObject(object receivedObject, NetworkAddress networkAddress) {
            var signallingMessage = (SignallingMessage) receivedObject;
            Send(signallingMessage, signallingMessage.DestinationAddress);
        }
    }
}
