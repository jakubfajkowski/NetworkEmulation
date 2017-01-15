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
        private Queue<SubnetworkPointPool> snpPools;

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.ConnectionRequestCC:
                    message.Operation = SignallingMessageOperation.RouteTableQuery;
                    SendMessage(message);
                    break;
                case SignallingMessageOperation.RouteTableQueryResponse:
                    HandleRouteTableQueryResponse(message);
                    break;
                case SignallingMessageOperation.ConnectionRequestResponseCC:

                    break;
                case SignallingMessageOperation.SetLabels:
                    var labels = (int[]) message.Payload;
                    Debug.WriteLine("Received VPI: " + labels[0] + ", VCI: " + labels[1]);
                    break;
                case SignallingMessageOperation.GetLabelsFromLRM:
                    break;
            }
        }

        public void SendGetLabelsMessage()
        {
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
            if (snpPools.Count != 0) {
                snpPools = msg.Payload as Queue<SubnetworkPointPool>;
                var snpps = new SubnetworkPointPool[2] {
                    snpPools.Dequeue(),
                    snpPools.Dequeue()
                };
                //var destinationAddress = snpps[0].NetworkSnppAddress.GetId(msg.DestinationAddress.);
                //msg.DestinationAddress = destinationAddress;
            }
        }

        public void ConnectionRequest(SubnetworkPointPool snppA, SubnetworkPointPool snppB) {
            //var snpps = new List<SubnetworkPointPool> {snppA, snppB};
            //var signallingMessage = new SignallingMessage(SignallingMessageOperation.ConnectionRequestCC, snpps);
            //SendMessage(signallingMessage);
        }

        public void LinkConnectionRequest() {
        }
    }
}