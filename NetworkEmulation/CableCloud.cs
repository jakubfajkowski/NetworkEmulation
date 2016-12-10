using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NetworkUtilities;

namespace NetworkEmulation {
    [XmlRoot("CableCloud")]
    public class CableCloud : LogObject, IXmlSerializable {
        private UdpClient _connectionUdpClient;
        [XmlElement("Links")]
        private SerializableDictionary<SocketNodePortPair, SocketNodePortPair> _linkDictionary;
        private readonly SerializableDictionary<int, TcpClient> _nodesTcpClients;
        public bool Online { get; private set; }

        public CableCloud() {
            _nodesTcpClients = new SerializableDictionary<int, TcpClient>();
            _linkDictionary = new SerializableDictionary<SocketNodePortPair, SocketNodePortPair>();

            Start();
        }

        private void Start() {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 10000);
            _connectionUdpClient = new UdpClient(ipEndPoint);

            ListenForConnectionRequests();
        }

        private void ListenForConnectionRequests() {
            Task.Run(async () => {
                using (_connectionUdpClient) {
                    Online = true;
                    while (true) {
                        var receivedData = await _connectionUdpClient.ReceiveAsync();
                        EstabilishNodeConnection(BitConverter.ToInt32(receivedData.Buffer, 0));
                    }
                }
            });
        }

        private void EstabilishNodeConnection(int port) {
            var nodeTcpClient = new TcpClient();
            try {
                nodeTcpClient.Connect(IPAddress.Loopback, port);
                _nodesTcpClients.Add(port, nodeTcpClient);
                UpdateStatus("Connected to Node on port: " + port);
                ListenForNodeMessages(nodeTcpClient, port);
            }
            catch (SocketException e) {
                UpdateStatus(e.Message);
            }
        }

        private void ListenForNodeMessages(TcpClient nodeTcpClient, int inputPort) {
            Task.Run(async () => {
                using (var ns = nodeTcpClient.GetStream()) {
                    var buffer = new byte[CableCloudMessage.MaxByteBufferSize];

                    while (true) {
                        var bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead <= 0)
                            break;

                        var cableCloudMessage = CableCloudMessage.deserialize(buffer);
                        UpdateStatus("Router " + inputPort + ": " + cableCloudMessage.portNumber + " - message recieved.");
                        var input = new SocketNodePortPair(cableCloudMessage.portNumber, inputPort);
                        var output = LookUpLinkDictionary(input);
                        cableCloudMessage.portNumber = output.NodePortNumber;

                        PassCableCloudMessage(cableCloudMessage, output.SocketPortNumber);
                    }
                }
            });
        }

        private SocketNodePortPair LookUpLinkDictionary(SocketNodePortPair input) {
            return _linkDictionary[input];
        }

        private void PassCableCloudMessage(CableCloudMessage cableCloudMessage, int outputPort) {
            try {
                var tcpClient = _nodesTcpClients[outputPort];

                SendBytes(CableCloudMessage.serialize(cableCloudMessage), tcpClient);
                UpdateStatus("Router " + outputPort + ": " + cableCloudMessage.portNumber + " - message sent.");
            }
            catch (KeyNotFoundException) {
                UpdateStatus("Router " + outputPort + ": " + cableCloudMessage.portNumber + " - no avaliable link.");
            }
        }

        private void SendBytes(byte[] data, TcpClient tcpClient) {
            tcpClient.GetStream().Write(data, 0, data.Length);
        }

        public void AddLink(SocketNodePortPair key, SocketNodePortPair value) {
            _linkDictionary.Add(key, value);
        }

        public void RemoveLink(SocketNodePortPair key) {
            _linkDictionary.Remove(key);
        }

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            XmlSerializer linkSerializer = new XmlSerializer(_linkDictionary.GetType());

            reader.ReadStartElement("CableCloud");
            reader.ReadStartElement("Links");
            _linkDictionary = (SerializableDictionary<SocketNodePortPair, SocketNodePortPair>) linkSerializer.Deserialize(reader);
            reader.ReadEndElement();
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            XmlSerializer linkSerializer = new XmlSerializer(_linkDictionary.GetType());

            writer.WriteStartElement("Links");
            linkSerializer.Serialize(writer, _linkDictionary);
            writer.WriteEndElement();
        }
    }
}