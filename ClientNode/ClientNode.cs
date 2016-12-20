using System.Collections.Generic;
using NetworkUtilities;
using NetworkUtilities.element;

namespace ClientNode {
    public class ClientNode : Node {
        public delegate void MessageHandler(object sender, string text);

        public ClientNode(ClientNodeSerializableParameters parameters) : base(parameters.IpAddress, parameters.CableCloudListeningPort, parameters.CableCloudDataPort) {
            ClientName = parameters.ClientName;
        }

        public string ClientName { get; private set; }
        public List<ClientTableRow> ClientTableList = new List<ClientTableRow>();

        public event MessageHandler OnUpdateState;
        public event MessageHandler OnMessageRecieved;
        public event MessageHandler OnNewClientTableRow;

        public void ReadClientTable(ClientNodeSerializableParameters parameters) {
            if (parameters.ClientTable != null) {
                foreach (var client in parameters.ClientTable) {
                    AddClient(client);
                }
            }
        }

        protected void UpdateState(string state) {
            OnUpdateState?.Invoke(this, state);
        }

        protected void MessageRecieved(string message) {
            OnMessageRecieved?.Invoke(this, message);
        }

        protected void AddClientToComboBox(string clientName) {
            OnNewClientTableRow?.Invoke(this, clientName);
        }

        public void AddClient(ClientTableRow clientTableRow) {
            ClientTableList.Add(clientTableRow);
            AddClientToComboBox(clientTableRow.ClientName);
        }

        public void SendMessage(string message, string recieverName) {
            var clientTableRow = SearchUsingClientName(recieverName);

            if (clientTableRow != null) SendCableCloudMessage(message, clientTableRow);
            else UpdateState("Client " + recieverName + " not found.");
        }

        private ClientTableRow SearchUsingClientName(string clientName) {
            foreach (var clientTableRow in ClientTableList)
                if (clientTableRow.ClientName.Equals(clientName)) return clientTableRow;
            return null;
        }

        private void SendCableCloudMessage(string message, ClientTableRow clientTableRow) {
            var vci = clientTableRow.Vci;
            var vpi = clientTableRow.Vpi;
            var portNumber = clientTableRow.PortNumber;

            var cableCloudMessage = new CableCloudMessage(portNumber, vpi, vci, message);
            Send(cableCloudMessage.Serialize());
            UpdateState("Sent: " + cableCloudMessage.AtmCells.Count + " ATMCells.");
        }

        protected override void HandleMessage(CableCloudMessage cableCloudMessage) {
            MessageRecieved(cableCloudMessage.ToString());
            UpdateState("Recieved: " + cableCloudMessage.AtmCells.Count + " ATMCells.");
        }
    }
}