﻿using System;
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
        public int agentPort;
        private TcpListener agentTcpListener;
        public int cloudPort;
        private TcpListener cloudTcpListener;

        private Node() {
        }

        /**
        * Konstruktor klasy Node wczytujący porty agenta i chmury i tworzący na ich podstawie @cloudSocket i @networkManagerSocket, po czym tworzone jest połączenie z chmurą
        */

        public Node(int agentPort, int cloudPort) {
            this.cloudPort = cloudPort;
            this.agentPort = agentPort;
            try {
                cloudTcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), this.cloudPort);
                agentTcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), this.agentPort);
            }
            catch (Exception e) {
                Debug.Fail(e.ToString(),
                    string.Format("Can't connect to port {0} or port {1}!", this.cloudPort, this.agentPort));
            }
            listenForConnectRequest(cloudTcpListener);
            // listenForConnectRequest(agentTcpListener);
            connectToCloud();
        }

        private void listenForConnectRequest(TcpListener tcpListener) {
            tcpListener.Start();
            Task.Run(async () => {
                listenForNodeMessages(await tcpListener.AcceptTcpClientAsync());
                online = true;
            });
        }

        private void listenForNodeMessages(TcpClient nodeTcpClient) {
            Task.Run(async () => {
                using (NetworkStream ns = nodeTcpClient.GetStream()) {
                    MemoryStream ms = new MemoryStream();
                    byte[] buffer = new byte[1024];

                    while (true) {
                        int bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead <= 0)
                            break;
                        ms.Write(buffer, 0, bytesRead);
                        handleMessage(ms.ToArray());
                        ms.Seek(0, SeekOrigin.Begin);
                    }
                }
            });
        }

        protected void handleMessage(byte[] data) {
            Console.Write(data);
        }

        /**
        * Metoda wysyłająca cloudPort do chmury, na którym będzie nasłuchiwać
        */

        private void connectToCloud() {
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

        public bool isOnline() {
            return online;
        }
    }
}