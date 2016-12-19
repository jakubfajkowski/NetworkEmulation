using System.Collections.Generic;
using NetworkUtilities;
using NetworkUtilities.element;

namespace ClientNode {
    public class ClientNode : Node {
        public delegate void MessageHandler(object sender, string text);

        public ClientNode(ClientNodeSerializableParameters param) {
            Id = param.Id;
            ClientName = param.ClientName;
            CloudPort = param.CloudPort;
            IpAddress = param.IpAddress;
            foreach(var client in param.ClientTable) {
                AddClient(client);
            }
        }

        public int Id { get; set; }
        public string ClientName { get; set; }
        public int CloudPort { get; set; }
        //public int ClientPort { get; set; }
        public string IpAddress { get; set; }
        public List<ClientTableRow> ClientTableList = new List<ClientTableRow>();

        public event MessageHandler OnUpdateState;
        public event MessageHandler OnMessageRecieved;

        protected void UpdateState(string state) {
            OnUpdateState?.Invoke(this, state);
        }

        protected void MessageRecieved(string message) {
            OnMessageRecieved?.Invoke(this, message);
        }

        public void AddClient(string clientName, int clientPort, int vpi, int vci) {
            ClientTableList.Add(new ClientTableRow(clientName, clientPort, vpi, vci));
        }

        public void AddClient(ClientTableRow clientTableRow) {
            AddClient(clientTableRow.ClientName, clientTableRow.PortNumber, clientTableRow.Vpi, clientTableRow.Vci);
        }

        public void SendMessage(string message, string recieverName) {
            var clientTableRow = SearchUsingClientName(recieverName);

            if (clientTableRow != null) SendCableCloudMessage(message, clientTableRow);
            else UpdateState("Client " + recieverName + " not found.");
        }

        private ClientTableRow SearchUsingClientName(string clientName) {
            foreach (var clientTableRow in ClientTable)
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