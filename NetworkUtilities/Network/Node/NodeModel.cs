﻿using NetworkUtilities.Utilities;

namespace NetworkUtilities.Network.Node {
    public class NodeModel {
        public NetworkAddress NetworkAddress { get; set; }
        public int MaxAtmCellsNumberInCableCloudMessage { get; set; }
        public string IpAddress { get; set; }
        public int CableCloudListeningPort { get; set; }
        public int SignallingCloudListeningPort { get; set; }
    }
}