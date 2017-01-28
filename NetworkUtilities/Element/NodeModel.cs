namespace NetworkUtilities.Element {
    public class NodeModel {
        public NetworkAddress NetworkAddress { get; set; }
        public int MaxAtmCellsNumberInCableCloudMessage { get; set; }
        public string IpAddress { get; set; }
        public int CableCloudListeningPort { get; set; }
        public int CableCloudDataPort { get; set; }
        public int PathComputationServerListeningPort { get; set; }
        public int PathComputationServerDataPort { get; set; }
    }
}