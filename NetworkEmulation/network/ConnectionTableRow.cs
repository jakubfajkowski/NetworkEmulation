namespace NetworkEmulation.Network {
    public class ConnectionTableRow {
        public ConnectionTableRow(int node1, int node2, float capacity, float maxCapacity, int inputPort, int outputPort) {
            Node1InputPort = node1;
            Node2InputPort = node2;
            Capacity = capacity;
            MaxCapacity = maxCapacity;
            InputPort = inputPort;
            OutputPort = outputPort;
        }

        public int Node1InputPort { get; }
        public int Node2InputPort { get; }
        public float Capacity { get; private set; }
        public float MaxCapacity { get; }
        public int InputPort { get; private set; }
        public int OutputPort { get; private set; }

        public bool ChangeCapacity(float newCapacity) {
            if (Capacity + newCapacity <= MaxCapacity) {
                Capacity = Capacity + newCapacity;
                return true;
            }
            return false;
        }

        // Sprawdza czy dane połączenie ma punkty końcowe w węzłach node1 i node2
        public bool CheckNodes(int node1, int node2) {
            return (node1 == Node1InputPort) && (node2 == Node2InputPort);
        }
    }
}