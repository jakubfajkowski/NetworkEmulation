using System;
using System.Collections.Generic;
using System.Linq;
using NetworkUtilities.GraphAlgorithm;

namespace NetworkUtilities.ControlPlane {
    public class RoutingController : ControlPlaneElement {
        private readonly List<Link> _links = new List<Link>();

        public RoutingController(NetworkAddress networkAddress) : base(networkAddress) {}

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.RouteTableQuery:
                    var snpps = message.Payload as SubnetworkPointPool[];
                    //if (snpps != null) HandleRouteTableQuery(snpps[0], snpps[1], capacity, message);
                    break;
                case SignallingMessageOperation.LocalTopology:
                    _links.Add((Link) message.Payload);
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
            networkTopology.Payload = _links;
            //networkTopology.DestinationAddress = ???
            SendMessage(networkTopology);
        }

        //private SubnetworkPointPool[] Route(SubnetworkPointPool snppStart, SubnetworkPointPool snppEnd, int capacity) {
        //    var preparedList = _links.Where(link => link.Capacity >= capacity).ToArray();
        //    var graph = new Graph();
        //    graph.Load(preparedList);
        //    var paths = Floyd.RunAlgorithm(graph, snppStart, snppEnd);
        //    return paths[0].SubnetworkPointPools;
        //}

        private void HandleRouteTableQuery(SubnetworkPointPool snppStart, SubnetworkPointPool snppEnd, int capacity,
            SignallingMessage message) {
            message.Operation = SignallingMessageOperation.RouteTableQueryResponse;
            //message.Link = Route(snppStart, snppEnd, capacity);
            SendMessage(message);
        }
    }
}