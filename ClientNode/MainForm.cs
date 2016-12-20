using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetworkUtilities;
using NetworkUtilities.element;

namespace ClientNode {
    public partial class MainForm : Form {
        private readonly ClientNode _client;

        public MainForm(string[] args) {
            InitializeComponent();
            string joinedArgs = string.Join(" ", args);
            var param = (ClientNodeSerializableParameters)XmlSerializer.Deserialize(joinedArgs, typeof(ClientNodeSerializableParameters));
            _client = new ClientNode(param);

            this.Text = _client.ClientName;

            _client.OnUpdateState += UpdateState;
            _client.OnMessageRecieved += MessageRecieved;
            _client.OnNewClientTableRow += AddClientToComboBox;

            _client.ReadClientTable(param);
        }

        public void AddClientToComboBox(object sender, string clientName) {
            comboBoxClients.Items.Add(clientName);
        }

        private void buttonSend_Click(object sender, EventArgs e) {
            var message = textBoxMessage.Text;
            var recieverName = comboBoxClients.SelectedItem as string;

            _client.SendMessage(message, recieverName);
        }

        private void MessageRecieved(object sender, string message) {
            textBoxReceived.Text += message;
            textBoxReceived.Text += '\n';
        }

        private void UpdateState(object sender, string state) {
            textBoxEventLog.Text += CreateLogLine(state);
        }

        private string CreateLogLine(string text) {
            return $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] {text}\n";
        }

        private void buttonClear_Click(object sender, EventArgs e) {
            textBoxReceived.Clear();
        }
    }
}