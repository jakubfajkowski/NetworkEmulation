using System;

namespace NetworkUtilities.ControlPlane {
    [Serializable]
    class SubnetworkPointPortPair {
        public SubnetworkPoint SubnetworkPoint { get; private set; }
        public int Port { get; private set; }

        public SubnetworkPointPortPair(SubnetworkPoint subnetworkPoint, int port) {
            SubnetworkPoint = subnetworkPoint;
            Port = port;
        }

        public override string ToString() {
            return $"SubnetworkPoint: {SubnetworkPoint}, Port: {Port}";
        }
    }
}