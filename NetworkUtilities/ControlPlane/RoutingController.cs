using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkUtilities.Element;
using NetworkUtilities.GraphAlgorithm;

namespace NetworkUtilities.ControlPlane {
    class RoutingController : ControlPlaneElement {
        private List<Link> _linkList = new List<Link>();

        public RoutingController(NetworkAddress networkAddress) : base(networkAddress) {}

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.RouteTableQuery:
                    var snpps = message.Payload as List<SubnetworkPointPool>;
                    if (snpps != null) Route(snpps[0], snpps[1]);
                    break;
                case SignallingMessageOperation.LocalTopology:
                    _linkList.Add((Link)message.Payload);
                    break;
                case SignallingMessageOperation.NetworkTopology:

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SendNetworkTopology(SignallingMessage message) {
            var networkTopology = message;
            networkTopology.Operation = SignallingMessageOperation.NetworkTopology;
            networkTopology.Payload = _linkList;
            //networkTopology.DestinationAddress = ???
            SendMessage(networkTopology);
        }

        private void Route(SubnetworkPointPool snppA, SubnetworkPointPool snppB) {

        }
    }
}