using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetworkUtilities {
    [Serializable]
    public class Node {
        private bool online = false;
        public int cloudPort { get; }
        private TcpListener cloudTcpListener;
        private TcpClient nodeTcpClient;

        /**
        * Konstruktor klasy Node wczytujący porty agenta i chmury i tworzący na ich podstawie @cloudSocket i @networkManagerSocket, po czym tworzone jest połączenie z chmurą
        */

        public Node() {
            this.cloudPort = freeTcpPort();
            cloudTcpListener = createTcpListener(IPAddress.Loopback, cloudPort);
            listenForConnectRequest(cloudTcpListener);
            connectToCloud();
        }

        protected int freeTcpPort() {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint) l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        protected TcpListener createTcpListener(IPAddress ipAddress, int port) {
            TcpListener tcpListener = null;
            try {
                tcpListener = new TcpListener(ipAddress, port);
            }
            catch (Exception e) {
                Debug.Fail(e.ToString(),
                    string.Format("Can't connect to port {0}!", this.cloudPort));
            }

            return tcpListener;
        }

        protected void listenForConnectRequest(TcpListener tcpListener) {
            tcpListener.Start();
            Task.Run(async () => {
                nodeTcpClient = await tcpListener.AcceptTcpClientAsync();
                listenForNodeMessages();
                online = true;
            });
        }

        private void listenForNodeMessages() {
            Task.Run(async () => {
                using (NetworkStream ns = nodeTcpClient.GetStream()) {
                    byte[] buffer = new byte[CableCloudMessage.MaxByteBufferSize];

                    while (true) {
                        int bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length);
                        //Console.WriteLine("jest wiadomosc");
                        if (bytesRead <= 0)
                            break;

                        //Debug.WriteLine("NaszBufor: " + Encoding.ASCII.GetString(buffer));                      
                        handleMessage(CableCloudMessage.deserialize(buffer));
                    }
                }
            });
        }

        protected virtual void handleMessage(CableCloudMessage message) {
            //Console.WriteLine("Bazowa");
        }


        public void send(byte[] data) {
            Task.Run(() => {
                try {
                    // foreach (byte t in data)
                    //     tcpClient.GetStream().WriteByte(t);
                    nodeTcpClient.GetStream().Write(data, 0, data.Length);
                    Debug.WriteLine("SendData: " + Encoding.ASCII.GetString(data) + " dd");
                }
                catch {
                    Debug.WriteLine("Sending ERROR!");
                }
            });
        }

        /**
        * Metoda wysyłająca cloudPort do chmury, na którym będzie nasłuchiwać
        */

        private void connectToCloud() {
            var udpClient = new UdpClient();
            var bytesToSend = BitConverter.GetBytes(cloudPort);
            var ipEndpoint = new IPEndPoint(IPAddress.Loopback, 10000);
            udpClient.Send(bytesToSend, bytesToSend.Length, ipEndpoint);
        }

        /*  /**
            * Metoda generuje IDEndPoint na postawie podanego portu
            #1#
        private IPEndPoint generateIPEndPoint(int port) {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            return ipEndPoint;
        }*/

        /**
         * Metoda generuje XML na postawie klasy
         */

        public string toXML() {
            var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(GetType());
            serializer.Serialize(stringwriter, this);
            return stringwriter.ToString();
        }

        public static Node LoadFromXMLString(string xmlText) {
            var stringReader = new StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(Node));
            return serializer.Deserialize(stringReader) as Node;
        }

        public bool isOnline() {
            return online;
        }
    }
}