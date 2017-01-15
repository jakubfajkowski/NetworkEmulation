using System;
using System.Windows.Forms;
using NetworkUtilities;
using NetworkUtilities.Element;
using NetworkUtilities.Serialization;

namespace ClientNode {
    public partial class MainForm : Form {
        private readonly ClientNode _client;

        public MainForm(string[] args) {
            InitializeComponent();
            var joinedArgs = string.Join(" ", args);
            var param =
                (ClientNodeModel)
                XmlSerializer.Deserialize(joinedArgs, typeof(ClientNodeModel));
            _client = new ClientNode(param);

            Text = $"Client Node ({_client.ClientName})";

            textBoxEventLog.TextChanged += textBox_enableAutoscroll;
            textBoxReceived.TextChanged += textBox_enableAutoscroll;

            _client.UpdateState += UpdateState;
            _client.OnMessageRecieved += MessageRecieved;
            _client.OnNewClientTableRow += AddClientToComboBox;

            _client.ReadClientTable(param);
        }

        private void textBox_enableAutoscroll(object sender, EventArgs e) {
            var textBox = sender as TextBox;
            if (textBox == null) {
                return;
            }

            textBox.SelectionLength = textBox.Text.Length;
            textBox.ScrollToCaret();
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
            textBoxReceived.AppendText(message);
        }

        private void UpdateState(object sender, string state) {
            textBoxEventLog.AppendText(CreateLogLine(state));
        }

        private string CreateLogLine(string text) {
            return $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] {text}\n";
        }

        private void buttonClear_Click(object sender, EventArgs e) {
            textBoxReceived.Clear();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            Environment.Exit(0);
        }
    }
}