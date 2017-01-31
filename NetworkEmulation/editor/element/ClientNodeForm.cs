using System;
using System.Windows.Forms;
using NetworkEmulation.Properties;
using NetworkUtilities.Network.ClientNode;

namespace NetworkEmulation.Editor.Element {
    public partial class ClientNodeForm : Form {
        private readonly ClientNodeModel _parameters;

        public ClientNodeForm(ClientNodeView clientNodeView) {
            InitializeComponent();
            _parameters = clientNodeView.Parameters;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            _parameters.IpAddress = Settings.Default.IpAddress;
            _parameters.ClientName = textBoxClientName.Text;
            _parameters.CableCloudListeningPort = Settings.Default.CableCloudListenerPort;
            _parameters.SignallingCloudListeningPort = Settings.Default.SignallingCloudListeningPort;

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }
    }
}