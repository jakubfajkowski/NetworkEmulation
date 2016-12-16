using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NetworkEmulation.log;
using NetworkUtilities;

namespace NetworkEmulation.network {
    [XmlRoot("CableCloud")]
    public class CableCloud : LogObject, IXmlSerializable {
        private readonly SerializableDictionary<int, TcpClient> _nodesTcpClients;
        private UdpClient _connectionUdpClient;

        [XmlElement("Links")] private SerializableDictionary<SocketNodePortPair, SocketNodePortPair> _linkDictionary;

        public CableCloud() {
            _nodesTcpClients = new SerializableDictionary<int, TcpClient>();
            _linkDictionary = new SerializableDictionary<SocketNodePortPair, SocketNodePortPair>();

            Start();
        }

        public bool Online { get; private set; }

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            var linkSerializer = new System.Xml.Serialization.XmlSerializer(_linkDictionary.GetType());

            reader.ReadStartElement("CableCloud");
            reader.ReadStartElement("Links");
            _linkDictionary =
                (SerializableDictionary<SocketNodePortPair, SocketNodePortPair>) linkSerializer.Deserialize(reader);
            reader.ReadEndElement();
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            var linkSerializer = new System.Xml.Serialization.XmlSerializer(_linkDictionary.GetType());

            writer.WriteStartElement("Links");
            linkSerializer.Serialize(writer, _linkDictionary);
            writer.WriteEndElement();
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
                        EstabilishNodeConnection((int) BinarySerializer.Deserialize(receivedData.Buffer));
                    }
                }
            });
        }

        private void EstabilishNodeConnection(int port) {
            var nodeTcpClient = new TcpClient();
            try {
                nodeTcpClient.Connect(IPAddress.Loopback, port);
                _nodesTcpClients.Add(port, nodeTcpClient);
                UpdateState("Connected to Node on port: " + port);
                ListenForNodeMessages(nodeTcpClient, port);
            }
            catch (SocketException e) {
                UpdateState(e.Message);
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

                        var cableCloudMessage = CableCloudMessage.Deserialize(buffer);
                        UpdateState("Router " + inputPort + ": " + cableCloudMessage.PortNumber +
                                    " - message recieved.");
                        var input = new SocketNodePortPair(cableCloudMessage.PortNumber, inputPort);

                        try {
                            var output = LookUpLinkDictionary(input);
                            cableCloudMessage.PortNumber = output.NodePortNumber;

                            PassCableCloudMessage(cableCloudMessage, output.SocketPortNumber);
                        }
                        catch (KeyNotFoundException) {
                            UpdateState("Router " + input + ": " + cableCloudMessage.PortNumber + " - no avaliable link.");
                        }
                    }
                }
            });
        }

        private SocketNodePortPair LookUpLinkDictionary(SocketNodePortPair input) {
            return _linkDictionary[input];
        }

        private void PassCableCloudMessage(CableCloudMessage cableCloudMessage, int outputPort) {
                var tcpClient = _nodesTcpClients[outputPort];

                SendBytes(CableCloudMessage.Serialize(cableCloudMessage), tcpClient);
                UpdateState("Router " + outputPort + ": " + cableCloudMessage.PortNumber + " - message sent.");
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
    }
}