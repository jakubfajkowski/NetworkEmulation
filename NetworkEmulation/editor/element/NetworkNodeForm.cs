using System;
using System.Windows.Forms;
using NetworkEmulation.Properties;
using NetworkUtilities.Element;

namespace NetworkEmulation.Editor.Element {
    public partial class NetworkNodeForm : Form {
        private readonly NetworkNodeModel _parameters;

        public NetworkNodeForm(NetworkNodeView networkNodeView) {
            InitializeComponent();
            _parameters = networkNodeView.Parameters;

            textBoxIpAddress.Text = Settings.Default.IpAddress;
            textBoxCloudPort.Text = Settings.Default.CableCloudUdpListenerPortNumber.ToString();
            textBoxNMSPort.Text = Settings.Default.NetworkManagmentSystemUdpListenerPortNumber.ToString();
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            _parameters.IpAddress = textBoxIpAddress.Text;
            _parameters.CableCloudListeningPort = int.Parse(textBoxCloudPort.Text);
            _parameters.NetworkManagmentSystemListeningPort = int.Parse(textBoxNMSPort.Text);
            _parameters.NumberOfPorts = int.Parse(textBoxNumberOfPorts.Text);

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }
    }
}