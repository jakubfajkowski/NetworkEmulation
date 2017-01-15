using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkEmulation.Editor.Element;
using NetworkUtilities;
using NetworkUtilities.Network;

namespace NetworkEmulation.Network {
    public class CableCloud : ConnectionManager {
        private readonly Dictionary<SocketNodePortPair, SocketNodePortPair> _linkDictionary;

        public CableCloud(int port) : base(port) {
            _linkDictionary = new Dictionary<SocketNodePortPair, SocketNodePortPair>();
        }

        protected override void HandleReceivedObject(object receivedObject, int inputPort) {
            var cableCloudMessage = (CableCloudMessage) receivedObject;
            OnUpdateState("Node [" + inputPort + "] from ATM port: " + cableCloudMessage.PortNumber + " - " +
                        cableCloudMessage.Data.Length + " bytes received.");

            var input = new SocketNodePortPair(cableCloudMessage.PortNumber, inputPort);
            SocketNodePortPair output = null;

            try {
                output = LookUpLinkDictionary(input);
                cableCloudMessage.PortNumber = output.NodePortNumber;

                PassCableCloudMessage(cableCloudMessage, output.SocketPortNumber);
            }
            catch (KeyNotFoundException) {
                OnUpdateState("Node [" + input.SocketPortNumber + "] to ATM port: " +
                            cableCloudMessage.PortNumber +
                            " - no avaliable link.");
            }
            catch (Exception) {
                if (output != null) DisconnectClient(output.SocketPortNumber);
                OnUpdateState("Node [" + input.SocketPortNumber + "] to ATM port: " +
                            cableCloudMessage.PortNumber +
                            " - could not connect.");
            }
        }

        private SocketNodePortPair LookUpLinkDictionary(SocketNodePortPair input) {
            return _linkDictionary[input];
        }

        private void PassCableCloudMessage(CableCloudMessage cableCloudMessage, int outputPort) {
            SendObject(cableCloudMessage, outputPort);
            OnUpdateState("Node [" + outputPort + "] to   ATM port: " + cableCloudMessage.PortNumber + " - " +
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