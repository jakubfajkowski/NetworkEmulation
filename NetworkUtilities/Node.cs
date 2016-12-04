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
        [XmlIgnore]
        Socket socket;
        [XmlElement(ElementName = "statusHostName")]
        string statusHostName;
        [XmlElement(ElementName = "port")]
        int port;

        public Node() {
            newSocket(3402);
        }

        public Node(int port)
        {
            newSocket(port);   
        }
        private void newSocket(int port)
        {
            this.port = port;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            statusHostName = Dns.GetHostName();       
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddress, port);         
            socket.Bind(endPoint);
         }
        public void serializeToXML()
        {
            XmlSerializer XML = new XmlSerializer(typeof(Node));

           (FileStream fs = new FileStream("testXNLserialize.xml", FileMode.OpenOrCreate))
           {
            //TextWriter fs = new StreamWriter("testXMLserialize.xml");
                XML.Serialize(fs, this);
            fs.Close();
           }
            

            /*using (FileStream fs= new FileStream("testXNLserialize.xml", FileMode.OpenOrCreate))
            {
                Node n = (Node)XML.Deserialize(fs);
                Debug.WriteLine("Port: {0} -----", n.getPort());

            }*/
        }
        public string getPort()
        {
            return socket.LocalEndPoint.ToString();
        }
    }
}
