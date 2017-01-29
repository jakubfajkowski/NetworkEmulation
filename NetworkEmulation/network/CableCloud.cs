using System;
using System.Collections.Generic;
using NetworkEmulation.Editor.Element;
using NetworkUtilities;
using NetworkUtilities.Network;

namespace NetworkEmulation.Network {
    public class CableCloud : ConnectionManager {
        private readonly Dictionary<NetworkAddressNodePortPair, NetworkAddressNodePortPair> _linkDictionary;

        public CableCloud(int listeningPort) : base(listeningPort) {
            _linkDictionary = new Dictionary<NetworkAddressNodePortPair, NetworkAddressNodePortPair>();
        }

        protected override void HandleReceivedObject(object receivedObject, NetworkAddress networkAddress) {
            var cableCloudMessage = (CableCloudMessage) receivedObject;
            OnUpdateState("Node [" + networkAddress + "] from ATM port: " + cableCloudMessage.PortNumber + " - " +
                        cableCloudMessage.Data.Length + " bytes received.");

            var input = new NetworkAddressNodePortPair(networkAddress, cableCloudMessage.PortNumber);
            NetworkAddressNodePortPair output = null;

            try {
                output = LookUpLinkDictionary(input);
                cableCloudMessage.PortNumber = output.NodePortNumber;

                PassCableCloudMessage(cableCloudMessage, output.NetworkAddress);
            }
            catch (KeyNotFoundException) {
                OnUpdateState("Node [" + input.NetworkAddress + "] to ATM port: " +
                            cableCloudMessage.PortNumber +
                            " - no avaliable link.");
            }
            catch (Exception) {
                if (output != null) DisconnectClient(output.NetworkAddress);
                OnUpdateState("Node [" + input.NetworkAddress + "] to ATM port: " +
                            cableCloudMessage.PortNumber +
                            " - could not connect.");
            }
        }

        private NetworkAddressNodePortPair LookUpLinkDictionary(NetworkAddressNodePortPair input) {
            return _linkDictionary[input];
        }

        private void PassCableCloudMessage(CableCloudMessage cableCloudMessage, NetworkAddress outputNetworkAddress) {
            SendObject(cableCloudMessage, outputNetworkAddress);
            OnUpdateState("Node [" + outputNetworkAddress + "] to   ATM port: " + cableCloudMessage.PortNumber + " - " +
                        cableCloudMessage.Data.Length + " bytes sent.");
        }

        public void AddLink(NetworkAddressNodePortPair key, NetworkAddressNodePortPair value) {
            _linkDictionary.Add(key, value);
        }

        public void AddLink(LinkView link) {
            var key = link.Parameters.InputNodePortPair;
            var value = link.Parameters.OutputNodePortPair;

            _linkDictionary.Add(key, value);
        }

        public void RemoveLink(NetworkAddressNodePortPair key) {
            _linkDictionary.Remove(key);
        }
    }
}