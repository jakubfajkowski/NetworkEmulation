using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
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
            textBoxEventLog.Text = joinedArgs + "\n";
        }

        private void MainForm_Load(object sender, EventArgs e) {
            _client.UpdateState += UpdateState;
            _client.OnMessageReceived += MessageReceived;
            _client.OnClientTableRowAdded += AddConnection;
            _client.OnClientTableRowDeleted += DeleteConnection;
            _client.Initialize();

            Text = $"Client Node {_client.ClientName} ({_client.NetworkAddress})";

            textBoxEventLog.TextChanged += textBox_enableAutoscroll;
            textBoxReceived.TextChanged += textBox_enableAutoscroll;
        }

        private void textBox_enableAutoscroll(object sender, EventArgs e) {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            textBox.SelectionLength = textBox.Text.Length;
            textBox.ScrollToCaret();
        }

        private void buttonSend_Click(object sender, EventArgs e) {
            var message = textBoxMessage.Text;
            var receiverName = comboBoxConnections.Text;

            _client.SendMessage(message, receiverName);
        }

        public void AddConnection(object sender, string clientName) {
            comboBoxConnections.Items.Add(clientName);
        }

        public void DeleteConnection(object sender, string clientName) {
            comboBoxConnections.Items.Remove(clientName);
        }

        private void MessageReceived(object sender, string message) {
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

        private void buttonConnection_Click(object sender, EventArgs e) {
            if (buttonConnection.Text.Equals("Connect")) {
                buttonConnection.BackColor = Color.Red;
                buttonConnection.Text = "Disconnect";
                _client.Connect(textBoxReceiver.Text, (int) numericUpDownCapacity.Value);
                textBoxReceiver.Text = "";
            }
            else {
                buttonConnection.BackColor = Color.Lime;
                buttonConnection.Text = "Connect";
                _client.Disconnect(comboBoxConnections.Text);
            }
        }
    }
}