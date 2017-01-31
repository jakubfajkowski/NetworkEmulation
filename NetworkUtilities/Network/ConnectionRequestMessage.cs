using System;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.Network {
    [Serializable]
    public class ConnectionRequestMessage {
        public ConnectionRequestMessage(NetworkAddress networkAddress, int socketPort) {
            NetworkAddress = networkAddress;
            SocketPort = socketPort;
        }

        public NetworkAddress NetworkAddress { get; private set; }
        public int SocketPort { get; private set; }
    }
}