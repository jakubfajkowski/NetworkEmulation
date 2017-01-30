using System;
using System.Windows.Forms;
using NetworkEmulation.Properties;
using NetworkUtilities.Element;

namespace NetworkEmulation.Editor.Element {
    public partial class ClientNodeForm : Form {
        private readonly ClientNodeModel _parameters;

        public ClientNodeForm(ClientNodeView clientNodeView) {
            InitializeComponent();
            _parameters = clientNodeView.Parameters;

            textBoxIpAddress.Text = Settings.Default.IpAddress;
            textBoxCloudPort.Text = Settings.Default.CableCloudListenerPort.ToString();
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            _parameters.IpAddress = textBoxIpAddress.Text;
            _parameters.ClientName = textBoxClientName.Text;
            _parameters.CableCloudListeningPort = int.Parse(textBoxCloudPort.Text);

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }
    }
}