using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetworkUtilities.DataPlane;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public class ConnectionController : ControlPlaneElement {
        public delegate void CommutationHandler(object sender, CommutationHandlerArgs args);

        private readonly Dictionary<UniqueId, NetworkAddress[]> _clientAddressDictionary =
            new Dictionary<UniqueId, NetworkAddress[]>();
        private readonly Dictionary<UniqueId, Queue<SubnetworkPointPool>> _snppDictionary =
            new Dictionary<UniqueId, Queue<SubnetworkPointPool>>();

        public ConnectionController(NetworkAddress networkAddress) : base(networkAddress, ControlPlaneElementType.CC) {}

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case OperationType.ConnectionRequest:
                    HandleConnectionRequestFromNCC(message);
                    break;

                case OperationType.PeerCoordination:
                    HandlePeerCoordination(message);
                    break;

                case OperationType.RouteTableQuery:
                    HandleRouteTableQuery(message);
                    break;

                case OperationType.ConnectionRequestResponse:
                    HandleConnectionRequestResponse(message);
                    break;

                case OperationType.LinkConnectionResponse:
                    HandleLinkConnectionResponse(message);
                    break;
            }
        }

        private void HandleConnectionRequestFromNCC(SignallingMessage message) {
            var clientAddresses = (SubnetworkPointPool[]) message.Payload;

            if (clientAddresses[1].NetworkAddress.DomainId != Address.DomainId) 
                SendPeerCoordination(message);
            else
                SendRouteTableQuery(message);
        }

        private void HandlePeerCoordination(SignallingMessage message) {
            SendRouteTableQuery(message);
        }

        private void SendPeerCoordination(SignallingMessage message) {
            var peerCoordination = message;
            peerCoordination.Operation = OperationType.PeerCoordination;
            peerCoordination.DestinationAddress = OtherDomainAddress();
            peerCoordination.DestinationControlPlaneElement = ControlPlaneElementType.CC;

            SendMessage(peerCoordination);
        }

        private void HandleRouteTableQuery(SignallingMessage message) {
            var snppQueue = (Queue<SubnetworkPointPool>) message.Payload;
            _snppDictionary.Add(message.SessionId, snppQueue);

            if (snppQueue.Count > 0) {
                var snpps = new[] {
                    snppQueue.Dequeue(),
                    snppQueue.Dequeue()
                };

                message.Payload = snpps;
                if (snpps[0].NetworkNodeAddress.Levels - Address.Levels != 1) {
                    SendConnectionRequest(message, snpps[0].NetworkNodeAddress);
                }
                else {
                    SendLinkConnectionRequest(message, snpps[0].NetworkNodeAddress);
                }
            }
            else {
                SendConnectionRequestResponse(message, false);
            }
        }

        private void HandleConnectionRequestResponse(SignallingMessage message) {
            var succeeded = (bool)message.Payload;

            if (succeeded)
                SendConnectionRequestResponse(message, true);

        }

        private void HandleLinkConnectionResponse(SignallingMessage message) {
        }

        private void SendRouteTableQuery(SignallingMessage message) {
            var routeTableQuery = message;
            routeTableQuery.Operation = OperationType.RouteTableQuery;
            routeTableQuery.DestinationAddress = Address;
            routeTableQuery.DestinationControlPlaneElement = ControlPlaneElementType.RC;

            SendMessage(routeTableQuery);
        }

        private void SendConnectionRequest(SignallingMessage message, NetworkAddress destinationAddress) {
            var connectionRequest = message; 
            connectionRequest.Operation = OperationType.ConnectionRequest;
            connectionRequest.DestinationAddress = destinationAddress;
            connectionRequest.DestinationControlPlaneElement = ControlPlaneElementType.CC;

            SendMessage(connectionRequest);
        }

        private void SendLinkConnectionRequest(SignallingMessage message, NetworkAddress destinationAddress) {
            var linkConnectionRequest = message;
            linkConnectionRequest.Operation = OperationType.SNPLinkConnectionRequest;
            linkConnectionRequest.DestinationAddress = destinationAddress;
            linkConnectionRequest.DestinationControlPlaneElement = ControlPlaneElementType.LRM;

            SendMessage(linkConnectionRequest);
        }

        private void SendConnectionRequestResponse(SignallingMessage message, bool succeeded) {
            var connectionRequest = message;
            connectionRequest.Payload = succeeded;
            connectionRequest.Operation = OperationType.ConnectionRequest;

            if (Address.Levels > 2) {
                connectionRequest.DestinationAddress = Address.GetParentsAddress();
                connectionRequest.DestinationControlPlaneElement = ControlPlaneElementType.CC;
            }
            else {
                connectionRequest.DestinationAddress = Address;
                connectionRequest.DestinationControlPlaneElement = ControlPlaneElementType.NCC;
            }

            SendMessage(message);
        }

        //private void HandleSetSnp(SignallingMessage message) {
        //    //snPoint = message.Payload as SubnetworkPoint;
        //    //Port = message.SourceAddress.GetLastId();
        //}




        //public void SendGetLabelsMessage() {
        //    //SendMessage(new SignallingMessage(SignallingMessageOperation.SNPLinkConnectionDeallocation, 1));
        //}



        public NetworkAddress OtherDomainAddress() {
            if (Address.DomainId == 1) return new NetworkAddress(2);
            if (Address.DomainId == 2) return new NetworkAddress(1);
            return null;
        }

        public event CommutationHandler CommutationCommand;
        protected virtual void OnCommutationCommand(CommutationHandlerArgs args) {
            CommutationCommand?.Invoke(this, args);
        }
    }

    public class CommutationHandlerArgs {
        public CommutationHandlerArgs(CommutationTableRow commutationTableRow) {
            CommutationTableRow = commutationTableRow;
        }

        public CommutationTableRow CommutationTableRow { get; private set; }
    }
}