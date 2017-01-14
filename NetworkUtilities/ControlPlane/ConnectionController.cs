using System;
using System.Collections.Generic;
using NetworkUtilities.Element;

namespace NetworkUtilities.ControlPlane {
    class ConnectionController : ControlPlaneElement {
        public void RouteTableQuery(SubnetworkPointPool snppA, SubnetworkPointPool snppB) {
        }

        public void ConnectionRequest(SubnetworkPointPool snppA, SubnetworkPointPool snppB) {
            var snpps = new List<SubnetworkPointPool> {snppA, snppB};
            var signallingMessage = new SignallingMessage(SignallingMessageOperation.ConnectionRequestCC, snpps);
            SendMessage(signallingMessage);
        }

        public void LinkConnectionRequest() {
        }


        public override void RecieveMessage(SignallingMessage message) {
            throw new NotImplementedException();
        }
    }
}