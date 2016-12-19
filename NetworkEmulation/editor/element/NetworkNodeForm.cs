using System;
using System.Windows.Forms;
using NetworkUtilities.element;

namespace NetworkEmulation.editor.element {
    public partial class NetworkNodeForm : Form {
        private readonly NetworkNodeSerializableParameters networkNodeSerializableParameters;

        public NetworkNodeForm(NetworkNodeSerializableParameters param) {
            InitializeComponent();
            networkNodeSerializableParameters = param;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            networkNodeSerializableParameters.IpAddress = textBoxIpAddress.Text;
            networkNodeSerializableParameters.CloudPort = int.Parse(textBoxCloudPort.Text);
            networkNodeSerializableParameters.NetworkManagmentSystemPort = int.Parse(textBoxNMSPort.Text);
            networkNodeSerializableParameters.NumberOfPorts = int.Parse(textBoxNumberOfPorts.Text);

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }
    }
}