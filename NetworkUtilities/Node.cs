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
        public int agentPort;
        private TcpListener agentTcpListener;
        private readonly byte[] bytes = new byte[256];
        public int cloudPort;
        private TcpListener cloudTcpListener;

        private Node() {
        }

        /**
        * Konstruktor klasy Node wczytujący porty agenta i chmury i tworzący na ich podstawie @cloudSocket i @networkManagerSocket, po czym tworzone jest połączenie z chmurą
        */

        public Node(int portA, int portC) {
            cloudPort = portC;
            agentPort = portA;
            try {
                cloudTcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), cloudPort);
                agentTcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), agentPort);
            }
            catch (Exception e) {
                Debug.Fail(e.ToString(),
                    string.Format("Can't connect to port {0} or port {1}!", cloudPort, agentPort));
            }
            startListening(cloudTcpListener);
            // startListening(agentTcpListener);
            connectToCloud();
        }

        /**
        * Metoda generująca @cloudSocket na podstawie portu @cloudPort 
        */

        public void updateCloudListener() {
            cloudTcpListener = updateListener(cloudTcpListener, cloudPort);
        }

        /**
        * Metoda generująca @agentTcpListener na podstawie portu @agentPort
        */

        public void updateAgentTCPListener() {
            agentTcpListener = updateListener(agentTcpListener, agentPort);
        }

        /**
        * Metoda na danym etapie nasłuchuje na wchodzącą wiadomość i w razie przyjścia obsługuje go.(wysyła odpowiedź)
        */

        private void startListening(TcpListener tcpListener) {
            string data = null;
            Task.Run(() => {
                tcpListener.Start();
                while (true) {
                    Debug.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here.
                    var client = tcpListener.AcceptTcpClient();
                    Debug.WriteLine("Connected!");
                    data = null;
                    // Get a stream object for reading and writing
                    var stream = client.GetStream();
                    int i;
                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0) {
                        // Translate data bytes to a ASCII string.
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);
                        Debug.WriteLine("Received: " + data);

                        // Process the data sent by the client.
                        // data = data.ToUpper();

                        var msg = Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Debug.WriteLine("Sent: {0}", data);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            });
        }

        /**
        * Metoda która przypisuje port do socketa
        * @param socket socket, do którego przypisujemy port
        * @param port numer portu, na którym będzie działać socket
        */

        private TcpListener updateListener(TcpListener tcpListener, int port) {
            try {
                tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                return tcpListener;
            }
            catch (Exception e) {
                Debug.Fail(e.ToString(),
                    $"Can't connect to port {port}!");
                return null;
            }
        }

        /**
        * Metoda wysyłająca cloudPort do chmury, na którym będzie nasłuchiwać
        */

        public void connectToCloud() {
            var udpClient = new UdpClient();
            var bytesToSend = BitConverter.GetBytes(cloudPort);
            var ipEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
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
    }
}