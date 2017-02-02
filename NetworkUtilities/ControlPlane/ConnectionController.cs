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

        public ConnectionController(NetworkAddress localAddress) : base(localAddress, ControlPlaneElementType.CC) {}

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation) {
                case OperationType.ConnectionRequest:
                    if (message.SourceControlPlaneElement.Equals(ControlPlaneElementType.NCC))
                        HandleConnectionRequestFromNCC(message);
                    else if (message.SourceControlPlaneElement.Equals(ControlPlaneElementType.CC))
                        HandleConnectionRequestFromCC(message);
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

                case OperationType.SNPLinkConnectionRequest:
                    HandleSNPLinkConnectionRequest(message);
                    break;
            }
        }

        private void HandleConnectionRequestFromNCC(SignallingMessage message) {
            SendRouteTableQuery(message);
        }

        private void HandleConnectionRequestFromCC(SignallingMessage message) {
            if (message.Payload is bool) {
                var succeeded = (bool) message.Payload;
                SendConnectionRequestResponse(message, succeeded);
            }
            else {
                var snpps = (SubnetworkPointPool[]) message.Payload;

                if (snpps[0].NetworkNodeAddress.Equals(LocalAddress)) {
                    SendLinkConnectionRequest(message);
                }
                else {
                    SendRouteTableQuery(message);
                }
            }
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

                var childAddress = snpps[0].NetworkNodeAddress.GetRootFromBeginning(LocalAddress.Levels + 1);
                SendConnectionRequest(message, childAddress);
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

        private void HandleSNPLinkConnectionRequest(SignallingMessage message) {
            var r = (RecentSnp[]) message.Payload;

            var rowToAdd = new CommutationTableRow(r[0].SubnetworkPoint.Vpi,
                                                   r[0].SubnetworkPoint.Vci,
                                                   r[0].Port,
                                                   r[1].SubnetworkPoint.Vpi,
                                                   r[1].SubnetworkPoint.Vci,
                                                   r[1].Port);

            OnCommutationCommand(new CommutationHandlerArgs(rowToAdd));
        }

        private void SendRouteTableQuery(SignallingMessage message) {
            var routeTableQuery = message;
            routeTableQuery.Operation = OperationType.RouteTableQuery;
            routeTableQuery.DestinationAddress = LocalAddress;
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

        private void SendLinkConnectionRequest(SignallingMessage message) {
            var linkConnectionRequest = message;
            linkConnectionRequest.Operation = OperationType.SNPLinkConnectionRequest;
            linkConnectionRequest.DestinationAddress = LocalAddress;
            linkConnectionRequest.DestinationControlPlaneElement = ControlPlaneElementType.LRM;

            SendMessage(linkConnectionRequest);
        }

        private void SendConnectionRequestResponse(SignallingMessage message, bool succeeded) {
            var connectionRequest = message;
            connectionRequest.Payload = succeeded;
            connectionRequest.Operation = OperationType.ConnectionRequest;

            if (LocalAddress.IsDomain) {
                connectionRequest.DestinationAddress = LocalAddress;
                connectionRequest.DestinationControlPlaneElement = ControlPlaneElementType.NCC;
            }
            else {
                connectionRequest.DestinationAddress = LocalAddress.GetParentsAddress();
                connectionRequest.DestinationControlPlaneElement = ControlPlaneElementType.CC;
            }

            SendMessage(message);
        }

        public NetworkAddress OtherDomainAddress() {
            if (LocalAddress.DomainId == 1) return new NetworkAddress(2);
            if (LocalAddress.DomainId == 2) return new NetworkAddress(1);
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