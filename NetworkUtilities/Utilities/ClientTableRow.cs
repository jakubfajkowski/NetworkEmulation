namespace NetworkUtilities.Utilities {
    public class ClientTableRow {
        public ClientTableRow() {
        }

        public ClientTableRow(string clientName, int portNumber, int vpi, int vci) {
            ClientName = clientName;
            PortNumber = portNumber;
            Vpi = vpi;
            Vci = vci;
        }

        public string ClientName { get; set; }
        public int PortNumber { get; set; }
        public int Vpi { get; set; }
        public int Vci { get; set; }
    }
}