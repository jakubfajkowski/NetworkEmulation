using System;
using System.Windows.Forms;
using NetworkUtilities.element;

namespace NetworkEmulation.editor.element {
    public partial class NetworkNodeForm : Form {
        NetworkNodeSerializableParameters networkNodeSerializableParameters;
        public NetworkNodeForm(NetworkNodeSerializableParameters param) {
            InitializeComponent();
            networkNodeSerializableParameters = param;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            networkNodeSerializableParameters.Id = int.Parse(textBoxID.Text);
            networkNodeSerializableParameters.IpAddress = textBoxIpAddress.Text;
            networkNodeSerializableParameters.CloudPort = int.Parse(textBoxCloudPort.Text);
            networkNodeSerializableParameters.NetworkManagmentSystemPort = int.Parse(textBoxNMSPort.Text);
            networkNodeSerializableParameters.NumberOfPorts = int.Parse(textBoxNumberOfPorts.Text);

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
