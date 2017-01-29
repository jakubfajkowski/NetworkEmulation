using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkUtilities.ControlPlane {
    public class RoutingController : ControlPlaneElement {
        //public List<Link> LinkList = new List<Link>();

        public RoutingController(NetworkAddress networkAddress) : base(networkAddress) {
        }

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.RouteTableQuery:
                    var list = message.Payload as object[];
                    var snpps = list[0] as SubnetworkPointPool[];
                    var capacity = (int) list[1];
                    if (snpps != null) HandleRouteTableQuery(snpps[0], snpps[1], capacity, message);
                    break;
                case SignallingMessageOperation.LocalTopology:
                    //LinkList.Add((Link) message.Payload);
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
            //networkTopology.Payload = LinkList;
            //networkTopology.DestinationAddress = ???
            SendMessage(networkTopology);
        }

        //private SubnetworkPointPool[] Route(SubnetworkPointPool snppStart, SubnetworkPointPool snppEnd, int capacity) {
        //    var preparedList = LinkList.Where(link => link.Capacity >= capacity).ToArray();
        //    var graph = new Graph();
        //    graph.Load(preparedList);
        //    var paths = Floyd.RunAlgorithm(graph, snppStart, snppEnd);
        //    return paths[0].SubnetworkPointPools;
        //}

        private void HandleRouteTableQuery(SubnetworkPointPool snppStart, SubnetworkPointPool snppEnd, int capacity,
            SignallingMessage message) {
            message.Operation = SignallingMessageOperation.RouteTableQueryResponse;
            //message.Payload = Route(snppStart, snppEnd, capacity);
            SendMessage(message);
        }
    }
}