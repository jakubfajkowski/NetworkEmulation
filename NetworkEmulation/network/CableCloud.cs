using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkEmulation.Editor.Element;
using NetworkUtilities;
using NetworkUtilities.Serialization;

namespace NetworkEmulation.Network {
    public class CableCloud : ConnectionManager {
        private readonly SerializableDictionary<SocketNodePortPair, SocketNodePortPair> _linkDictionary;

        public CableCloud() {
            _linkDictionary = new SerializableDictionary<SocketNodePortPair, SocketNodePortPair>();
        }

        protected override Task Listen(TcpClient nodeTcpClient, int inputPort) {
            return new Task(async () => {
                using (var ns = nodeTcpClient.GetStream()) {
                    var buffer = new byte[CableCloudMessage.MaxByteBufferSize];

                    while (true) {
                        var bytesRead = await ns.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead <= 0)
                            break;

                        var cableCloudMessage = CableCloudMessage.Deserialize(buffer);
                        UpdateState("Node [" + inputPort + "] from ATM port: " + cableCloudMessage.PortNumber + " - " +
                                    cableCloudMessage.Data.Length + " bytes recieved.");
                        var input = new SocketNodePortPair(cableCloudMessage.PortNumber, inputPort);

                        SocketNodePortPair output = null;

                        try {
                            output = LookUpLinkDictionary(input);
                            cableCloudMessage.PortNumber = output.NodePortNumber;

                            PassCableCloudMessage(cableCloudMessage, output.SocketPortNumber);
                        }
                        catch (KeyNotFoundException) {
                            UpdateState("Node [" + input.SocketPortNumber + "] to ATM port: " +
                                        cableCloudMessage.PortNumber +
                                        " - no avaliable link.");
                        }
                        catch (Exception) {
                            if (output != null) NodesTcpClients.Remove(output.SocketPortNumber);
                            UpdateState("Node [" + input.SocketPortNumber + "] to ATM port: " +
                                        cableCloudMessage.PortNumber +
                                        " - could not connect.");
                        }
                    }
                }
            });
        }

        private SocketNodePortPair LookUpLinkDictionary(SocketNodePortPair input) {
            return _linkDictionary[input];
        }

        private void PassCableCloudMessage(CableCloudMessage cableCloudMessage, int outputPort) {
            var tcpClient = NodesTcpClients[outputPort];

            SendBytes(cableCloudMessage.Serialize(), tcpClient);
            UpdateState("Node [" + outputPort + "] to   ATM port: " + cableCloudMessage.PortNumber + " - " +
                        cableCloudMessage.Data.Length + " bytes sent.");
        }

        public void AddLink(SocketNodePortPair key, SocketNodePortPair value) {
            _linkDictionary.Add(key, value);
        }

        public void AddLink(Link link) {
            var key = link.Parameters.InputNodePortPair;
            var value = link.Parameters.OutputNodePortPair;

            _linkDictionary.Add(key, value);
        }

        public void RemoveLink(SocketNodePortPair key) {
            _linkDictionary.Remove(key);
        }
    }
}