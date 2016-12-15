namespace NetworkEmulation.network
{
    public class ConnectionTableRow
    {
        public int node1InputPort { get; private set; }
        public int node2InputPort { get; private set; }
        public float capacity { get; private set; }
        public float maxCapacity { get; }
        public int inputPort { get; private set; }
        public int outputPort { get; private set; }

        public ConnectionTableRow(int node1, int node2, float capacity, float maxCapacity, int inputPort, int outputPort)
        {
            this.node1InputPort = node1;
            this.node2InputPort = node2;
            this.capacity = capacity;
            this.maxCapacity = maxCapacity;
            this.inputPort = inputPort;
            this.outputPort = outputPort;
        }

        public bool changeCapacity(float newCapacity)
        {
            if (capacity + newCapacity <= maxCapacity)
            {
                capacity = capacity + newCapacity;
                return true;
            }
            else
                return false;
        }

        // Sprawdza czy dane połączenie ma punkty końcowe w węzłach node1 i node2
        public bool checkNodes(int node1, int node2)
        {
            return (node1 == node1InputPort) && (node2 == node2InputPort);
        }

    }
}
