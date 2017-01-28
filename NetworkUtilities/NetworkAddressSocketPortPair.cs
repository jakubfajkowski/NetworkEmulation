using System;

namespace NetworkUtilities {
    [Serializable]
    public class NetworkAddressSocketPortPair {
        public NetworkAddress NetworkAddress { get; private set; }
        public int SocketPort { get; private set; }

        public NetworkAddressSocketPortPair(NetworkAddress networkAddress, int socketPort) {
            NetworkAddress = networkAddress;
            SocketPort = socketPort;
        }
    }
}
