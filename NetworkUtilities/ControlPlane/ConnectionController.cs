using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NetworkUtilities.GraphAlgorithm;

namespace NetworkUtilities.ControlPlane {
    public class ConnectionController : ControlPlaneElement {
        private readonly int _networkNodeAgentUdpPort;

        private readonly Dictionary<UniqueId, NetworkAddress[]> _snppDictionary =
            new Dictionary<UniqueId, NetworkAddress[]>();

        private Queue<SubnetworkPointPool> _snpPools;
        private int _udpPortLrm;
        private int _udpPortRc;
        private Dictionary<string, int> _udpPortsCc;

        public ConnectionController(NetworkAddress networkAddress) : base(networkAddress) {
        }

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.ConnectionRequest:
                    var obj = (object[]) message.Payload;
                    var snpp = (SubnetworkPointPool[]) obj[0];
                    var snppAddressA = snpp[0].NetworkSnppAddress;
                    var snppAddressB = snpp[1].NetworkSnppAddress;
                    NetworkAddress[] snppAddress = {snppAddressA, snppAddressB};
                    _snppDictionary.Add(message.SessionId, snppAddress);

                    if (message.SourceAddress.Equals(new NetworkAddress("1")) ||
                        message.SourceAddress.Equals(new NetworkAddress("2"))) SendPeerCoordination(message);
                    else SendRouteTableQuery(message);
                    break;
                case SignallingMessageOperation.PeerCoordination:
                    SendRouteTableQuery(message);
                    break;
                case SignallingMessageOperation.RouteTableQueryResponse:
                    HandleRouteTableQueryResponse(message);
                    break;
                case SignallingMessageOperation.ConnectionRequestResponse:
                    HandleConnectionRequestResponse(message);
                    break;
                case SignallingMessageOperation.SetLabels:
                    var labels = (int[]) message.Payload;
                    Debug.WriteLine("Received VPI: " + labels[0] + ", VCI: " + labels[1]);
                    break;
                case SignallingMessageOperation.GetLabelsFromLRM:
                    break;
                case SignallingMessageOperation.ConnectionConfirmation:
                    if (message.DestinationAddress.Levels > 2) SendConnectionConfirmation(message);
                    else SendConnectionConfirmationToNCC(message);

                    break;
            }
        }

        private void SendLinkConnectionRequest(SignallingMessage message) {
            message.Operation = SignallingMessageOperation.LinkConnectionRequest;
            //message.DestinationAddress = ???
            message.DestinationControlPlaneElement = SignallingMessageDestinationControlPlaneElement.LinkResourceManager;
            SendMessage(message);
        }

        private void SendRouteTableQuery(SignallingMessage message) {
            message.Operation = SignallingMessageOperation.RouteTableQuery;
            message.DestinationAddress = _snppDictionary[message.SessionId][0];
            message.DestinationControlPlaneElement = SignallingMessageDestinationControlPlaneElement.RoutingController;
            SendMessage(message);
        }

        private void SendPeerCoordination(SignallingMessage message) {
            var peerCoordination = message;
            peerCoordination.Operation = SignallingMessageOperation.ConnectionConfirmation;
            if (message.SourceAddress.Equals(new NetworkAddress("1"))) {
                peerCoordination.DestinationAddress = new NetworkAddress("2");
                peerCoordination.DestinationControlPlaneElement =
                    SignallingMessageDestinationControlPlaneElement.ConnectionController;
            }
            else {
                peerCoordination.DestinationAddress = new NetworkAddress("1");
            }
            SendMessage(peerCoordination);
        }

        private void HandleConnectionRequestResponse(SignallingMessage msg) {
            if (_snpPools == null) {
                msg.DestinationAddress.GetParentsAddress();
                SendMessage(msg);
            }
            else if (Convert.ToBoolean(msg.Payload)) {
                HandleRouteTableQueryResponse(msg);
            }
            else {
                msg.DestinationAddress.GetParentsAddress();
                msg.Operation = SignallingMessageOperation.ConnectionRequestResponse;
                SendMessage(msg);
            }
        }

        private void SendConnectionConfirmation(SignallingMessage message) {
            var connectionConfirmation = message;
            connectionConfirmation.Operation = SignallingMessageOperation.ConnectionConfirmation;
            connectionConfirmation.Payload = true;
            connectionConfirmation.DestinationAddress = Address.GetParentsAddress();
            connectionConfirmation.DestinationControlPlaneElement =
                SignallingMessageDestinationControlPlaneElement.ConnectionController;
            SendMessage(connectionConfirmation);
        }

        private void SendConnectionConfirmationToNCC(SignallingMessage message) {
            message.Operation = SignallingMessageOperation.ConnectionConfirmation;
            message.DestinationAddress = _snppDictionary[message.SessionId][0].GetRootFromBeginning(1);
            message.DestinationControlPlaneElement =
                SignallingMessageDestinationControlPlaneElement.NetworkCallController;
        }

        public void SendGetLabelsMessage() {
            //SendMessage(new SignallingMessage(SignallingMessageOperation.GetLabels, 1));
        }

        public void SendConnectionToNetworkNodeAgent(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci,
            int outPortNumber) {
            var message = "CreateConnection " + inVpi + " " + inVci + " " + inPortNumber + " " + outVpi + " " + outVci +
                          " " + outPortNumber;
            var bytes = Encoding.UTF8.GetBytes(message);
            var udpClient = new UdpClient();
            var ipEndpoint = new IPEndPoint(IPAddress.Loopback, _networkNodeAgentUdpPort);
            udpClient.Send(bytes, bytes.Length, ipEndpoint);
        }

        private void HandleRouteTableQueryResponse(SignallingMessage msg) {
            if (msg.Operation.Equals(SignallingMessageOperation.RouteTableQueryResponse))
                _snpPools = msg.Payload as Queue<SubnetworkPointPool>;
            if (_snpPools != null) {
                var snpps = new[] {
                    _snpPools.Dequeue(),
                    _snpPools.Dequeue()
                };
                var levelsOfAddress = msg.DestinationAddress.Levels + 1;
                msg.Operation = SignallingMessageOperation.ConnectionRequest;
                msg.DestinationAddress = snpps[0].NetworkSnppAddress.GetRootFromBeginning(levelsOfAddress);
                msg.Payload = snpps;
                SendMessage(msg);
            }
            else {
                msg.Payload = false;
                msg.DestinationAddress.GetParentsAddress();
                msg.Operation = SignallingMessageOperation.ConnectionRequestResponse;
                SendMessage(msg);
            }
        }


        public void LinkConnectionRequest() {
        }
    }
}