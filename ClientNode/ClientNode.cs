using System.Collections.Generic;
using NetworkUtilities;

namespace ClientNode {
    public class ClientNode : Node {
        public delegate void MessageHandler(object sender, string text);

        private readonly int _portNumber = 10000;

        public ClientNode() {
            ClientTable = new List<ClientTableRow>();
        }

        public string ReceivedMessage { get; private set; }
        public string ClientName { get; set; }
        public List<ClientTableRow> ClientTable { get; set; }

        public event MessageHandler OnUpdateState;
        public event MessageHandler OnMessageRecieved;

        protected void UpdateState(string state) {
            OnUpdateState?.Invoke(this, state);
        }

        protected void MessageRecieved(string message) {
            OnMessageRecieved?.Invoke(this, message);
        }

        public void AddClient(string clientName, int portNumber, int vpi, int vci) {
            ClientTable.Add(new ClientTableRow(clientName, portNumber, vpi, vci));
        }

        public void SendMessage(string message, string recieverName) {
            var clientTableRow = SearchUsingClientName(recieverName);

            if (clientTableRow != null) {
                SendCableCloudMessage(message, clientTableRow);
            }
            else {
                UpdateState("Client " + recieverName + " not found.");
            }
            
        }

        private ClientTableRow SearchUsingClientName(string clientName) {
            foreach (var clientTableRow in ClientTable) {
                if (clientTableRow.ClientName.Equals(clientName)) return clientTableRow;
            }
            return null;
        }

        private void SendCableCloudMessage(string message, ClientTableRow clientTableRow) {
            var vci = clientTableRow.Vci;
            var vpi = clientTableRow.Vpi;
            var portNumber = clientTableRow.PortNumber;

            var cableCloudMessage = new CableCloudMessage(portNumber, vpi, vci, message);
            send(CableCloudMessage.serialize(cableCloudMessage));
            UpdateState("Sent: " + cableCloudMessage.atmCells.Count + "ATMCells.");
        }

        protected override void handleMessage(CableCloudMessage cableCloudMessage) {
            MessageRecieved(cableCloudMessage.ToString());
            UpdateState("Recieved: " + cableCloudMessage.atmCells.Count + "ATMCells.");
        }
    }
}