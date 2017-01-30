namespace NetworkUtilities.ManagementPlane {
    public class NodeConnectionInformation {
        public int NodeUdpPort { get; set; }
        public int InVpi { get; set; }
        public int InVci { get; set; }
        public int InPortNumber { get; set; }
        public int OutVpi { get; set; }
        public int OutVci { get; set; }
        public int OutPortNumber { get; set; }
    }
}