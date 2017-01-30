using System;

namespace NetworkUtilities.Utilities {
    [Serializable]
    public class NetworkAddressSocketPortPair {
        public NetworkAddressSocketPortPair(NetworkAddress networkAddress, int socketPort) {
            NetworkAddress = networkAddress;
            SocketPort = socketPort;
        }

        public NetworkAddress NetworkAddress { get; private set; }
        public int SocketPort { get; private set; }
    }
}