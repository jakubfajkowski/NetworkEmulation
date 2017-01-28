using System;
using System.Collections.Generic;
using NetworkUtilities.Element;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace NetworkUtilities.ControlPlane {
    public class ConnectionController : ControlPlaneElement {
        private readonly int _networkNodeAgentUdpPort;
        private int _udpPortLrm;

        private int _udpPortRc;
        private Dictionary<string, int> _udpPortsCc;
        private Queue<SubnetworkPointPool> _snpPools;


        public ConnectionController(NetworkAddress networkAddress) : base(networkAddress) {}

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.ConnectionRequest:
                    message.Operation = SignallingMessageOperation.RouteTableQuery;
                    SendMessage(message);
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
                    SendConnectionConfirmation(message);
                    break;
            }
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
            connectionConfirmation.Payload = (bool) true;
            connectionConfirmation.DestinationAddress = message.SourceAddress;
            SendMessage(connectionConfirmation);
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