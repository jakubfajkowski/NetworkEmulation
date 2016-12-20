using System.Net;
using System.Net.Sockets;

namespace NetworkUtilities {
    public static class PortRandomizer {
        public static int RandomFreePort() {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
