using System;
using System.Windows.Forms;
using NetworkEmulation.Properties;
using NetworkUtilities.element;

namespace NetworkEmulation.editor.element {
    public partial class ClientNodeForm : Form {
        private readonly ClientNodeSerializableParameters _parameters;

        public ClientNodeForm(ClientNodePictureBox clientNodePictureBox) {
            InitializeComponent();
            _parameters = clientNodePictureBox.Parameters;

            textBoxIpAddress.Text = Settings.Default.IpAddress;
            textBoxCloudPort.Text = Settings.Default.CableCloudUdpListenerPortNumber.ToString();
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