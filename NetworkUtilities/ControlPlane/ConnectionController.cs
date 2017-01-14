using System;
using System.Collections.Generic;
using NetworkUtilities.Element;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace NetworkUtilities.ControlPlane {
    public class ConnectionController : ControlPlaneElement {

        private int networkNodeAgentUdpPort;
        private int udpPortLRM;

        private int udpPortRC;
        private Dictionary<String, int> udpPortsCC;

        // Jeśli CC jest w NetworkNode
        public ConnectionController(int networkNodeAgentUdpPort, int udpPortLRM)
        {
            this.networkNodeAgentUdpPort = networkNodeAgentUdpPort;
            this.udpPortLRM = udpPortLRM;
        }


        public override void RecieveMessage(SignallingMessage message)
        {
            switch (message.Operation)
            {
                case SignallingMessageOperation.ConnectionRequestCC:
                    break;
                case SignallingMessageOperation.SetLabels:
                    int[] labels = (int[])message.Payload;
                    Debug.WriteLine("Received VPI: " + labels[0] + ", VCI: " + labels[1]);
                    break;
                case SignallingMessageOperation.GetLabelsFromLRM:
                    break;
            }
        }

        public void SendGetLabelsMessage()
        {
            SendMessage(new SignallingMessage(SignallingMessageOperation.GetLabels, 1));
        }

        public void SendConnectionToNetworkNodeAgent(int inVpi, int inVci, int inPortNumber, int outVpi, int outVci, int outPortNumber)
        {
            string message = "CreateConnection " + inVpi + " " + inVci + " " + inPortNumber + " " + outVpi + " " + outVci + " " + outPortNumber;
            var bytes = Encoding.UTF8.GetBytes(message);
            var udpClient = new UdpClient();
            var ipEndpoint = new IPEndPoint(IPAddress.Loopback, networkNodeAgentUdpPort);
            udpClient.Send(bytes, bytes.Length, ipEndpoint);
        }

        public void RouteTableQuery(SubnetworkPointPool snppA, SubnetworkPointPool snppB) {
        }

        public void ConnectionRequest(SubnetworkPointPool snppA, SubnetworkPointPool snppB) {
            var snpps = new List<SubnetworkPointPool> {snppA, snppB};
            var signallingMessage = new SignallingMessage(SignallingMessageOperation.ConnectionRequestCC, snpps);
            SendMessage(signallingMessage);
        }

        public void LinkConnectionRequest() {
        }


       
    }
}