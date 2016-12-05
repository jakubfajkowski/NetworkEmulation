using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtilities
{
    [Serializable]
    public class Node
    {
        public int cloudPort;
        public int agentPort;
        private Socket cloudSocket;
        private Socket networkManagerSocket;

        private Node() { }

        public Node(int portA, int portC) {
            cloudPort = portC;
            agentPort = portA;
            networkManagerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            cloudSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                networkManagerSocket.Bind(generateIPEndPoint(agentPort));
                cloudSocket.Bind(generateIPEndPoint(cloudPort));
                connectToCloud();
            } catch(Exception e){
                Debug.Fail(e.ToString(),
                string.Format("Can't connect to port {0} or port {1}!", cloudPort,agentPort));
            }
        }
        //po wczytaniu z XML metody update generują sokety na podstawie poertów
        public void updateCloudSocket() {
            cloudSocket = updateSocket(cloudSocket, cloudPort);
        }
        public void updateNetworkManagerSocket()
        {
            networkManagerSocket = updateSocket(networkManagerSocket, agentPort);
        }
        private Socket updateSocket(Socket socket, int port) {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Bind(generateIPEndPoint(port));
                return socket;
            }
            catch (Exception e)
            {
                Debug.Fail(e.ToString(),
                string.Format("Can't connect to port {0} or port {1]!", cloudPort, agentPort));
                return null;
            }
        }
        public void connectToCloud() {
            UdpClient udpClient = new UdpClient();
            byte[] bytesToSend = Encoding.ASCII.GetBytes("Port: " + cloudPort);
            var ipEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
        } 
        private IPEndPoint generateIPEndPoint(int port) {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            return ipEndPoint;
        }

        public string toXML() {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stringwriter, this);
            return stringwriter.ToString();
        }

        public static Node LoadFromXMLString(string xmlText) {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(Node));
            return serializer.Deserialize(stringReader) as Node;
        }
    }
}
