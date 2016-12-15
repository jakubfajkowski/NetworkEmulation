using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NetworkUtilities;

namespace ClientNode {
    public partial class MainForm : Form {
        private ClientNode _client;
        private List<ATMCell> _atmCells;
        private CableCloudMessage _cableCloudMessage;

        public MainForm(string[] args) {
            InitializeComponent();
            string.Join(" ", args);
            _client = new ClientNode();
            _client.OnUpdateState += UpdateState;
            _client.OnMessageRecieved += MessageRecieved;
        }

        public void AddClientToComboBox(string clientName) {
            comboBoxClients.Items.Add(clientName);
        }

        private void buttonSend_Click(object sender, EventArgs e) {
            var message = textBoxMessage.Text;
            var recieverName = comboBoxClients.SelectedItem as string;

            _client.SendMessage(message, recieverName);
        }

        private void MessageRecieved(object sender, string message) {
            textBoxReceived.Text += message;
        }

        private void UpdateState(object sender, string state) {
            textBoxEventLog.Text += CreateLogLine(state);
        }

        private string CreateLogLine(string text) {
            return $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] {text}\n";
        }
    }
}