namespace NetworkUtilities.element {
    public class NetworkNodeSerializableParameters : NodeSerializableParameters {
        public int NumberOfPorts { get; set; }
        public int NetworkManagmentSystemListeningPort { get; set; }
        public int NetworkManagmentSystemDataPort { get; set; }
    }
}