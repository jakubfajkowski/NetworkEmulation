namespace ClientNode {
    public class ClientTableRow {
        public string ClientName { get; set; }
        public int PortNumber { get; set; }
        public int Vpi { get; set; }
        public int Vci { get; set; }

        public ClientTableRow() {}

        public ClientTableRow(string clientName, int portNumber, int vpi, int vci) {
            ClientName = clientName;
            PortNumber = portNumber;
            Vpi = vpi;
            Vci = vci;
        }
    }
}
