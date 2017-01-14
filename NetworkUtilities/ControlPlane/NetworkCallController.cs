using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    class NetworkCallController : ControlPlaneElement {

        private void DirectoryRequest(string clientA, string clientZ) {
            Object[] clientName = {clientA, clientZ};
            SignallingMessage directioryRequest = new SignallingMessage(SignallingMessageOperation.DirectoryRequest, clientName);
            SendMessage(directioryRequest);
        }

        private void CallCoordination(string addressA, string addressZ ) {
            
        }

        private void ConnectionRequest(string addressA, string addressZ) {
            
        }

        public override void RecieveMessage(SignallingMessage message) {
            throw new NotImplementedException();
        }
    }
}
