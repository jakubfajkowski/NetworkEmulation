using System;
using System.Collections.Generic;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.DataPlane {
    public class CableCloud : ConnectionManager {
        private readonly Dictionary<NetworkAddressNodePortPair, NetworkAddressNodePortPair> _linkDictionary = 
            new Dictionary<NetworkAddressNodePortPair, NetworkAddressNodePortPair>();


        public CableCloud(int listeningPort) : 
            base(listeningPort, ConnectionManagerType.CableCloud) {}

        protected override void HandleReceivedObject(object receivedObject, NetworkAddress inputNetworkAddress) {
            var cableCloudMessage = (CableCloudMessage) receivedObject;
            var input = new NetworkAddressNodePortPair(inputNetworkAddress, cableCloudMessage.PortNumber);

            OnUpdateState($"[RECEIVED] {input.NetworkAddress}:{cableCloudMessage.PortNumber} OUT " +
                          $"{cableCloudMessage.Data.Length} BYTES");


            NetworkAddressNodePortPair output = null;

            try {
                output = LookUpLinkDictionary(input);
                cableCloudMessage.PortNumber = output.NodePortNumber;

                SendCableCloudMessage(cableCloudMessage, output.NetworkAddress);
            }
            catch (KeyNotFoundException) {
                OnUpdateState($"[NO_AVAILABLE_LINK] {input.NetworkAddress}:{cableCloudMessage.PortNumber} OUT");
            }
            catch (Exception) {
                if (output != null) {
                    OnUpdateState($"[CONNECTION_ERROR] {input.NetworkAddress}:{cableCloudMessage.PortNumber}");
                    DeleteConnection(output.NetworkAddress);
                }
            }
        }

        private NetworkAddressNodePortPair LookUpLinkDictionary(NetworkAddressNodePortPair input) {
            return _linkDictionary[input];
        }

        public void AddLink(NetworkAddressNodePortPair key, NetworkAddressNodePortPair value) {
            _linkDictionary.Add(key, value);
        }

        public void RemoveLink(NetworkAddressNodePortPair key) {
            _linkDictionary.Remove(key);
        }

        private void SendCableCloudMessage(CableCloudMessage cableCloudMessage, NetworkAddress outputNetworkAddress) {
            Send(cableCloudMessage, outputNetworkAddress);
            OnUpdateState($"[SENT] {outputNetworkAddress}:{cableCloudMessage.PortNumber} IN " +
                          $"{cableCloudMessage.Data.Length} BYTES");
        }
    }
}