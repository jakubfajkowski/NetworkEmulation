using System;
using System.Windows.Forms;
using NetworkEmulation.Properties;
using NetworkUtilities.Network.NetworkNode;

namespace NetworkEmulation.Editor.Element {
    public partial class NetworkNodeForm : Form {
        private readonly NetworkNodeModel _parameters;

        public NetworkNodeForm(NetworkNodeView networkNodeView) {
            InitializeComponent();
            _parameters = networkNodeView.Parameters;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            _parameters.IpAddress = Settings.Default.IpAddress;
            _parameters.CableCloudListeningPort = Settings.Default.CableCloudListenerPort;
            _parameters.NetworkManagmentSystemListeningPort = Settings.Default.NetworkManagmentSystemListeningPort;
            _parameters.SignallingCloudListeningPort = Settings.Default.SignallingCloudListeningPort;

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }
    }
}