using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities.Element;

namespace NetworkUtilities.ControlPlane {
    class RoutingController : ControlPlaneElement {
        private List<Link> _list;

        public override void RecieveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.RouteTableQuery:
                    var snpps = message.Payload as List<SubnetworkPointPool>;
                    if (snpps != null) Route(snpps[0], snpps[1]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Route(SubnetworkPointPool snppA, SubnetworkPointPool snppB) {
        }
    }
}

//