using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetworkUtilities;
using NetworkUtilities.element;

namespace NetworkEmulation.editor.element {
    public partial class ClientNodeForm : Form {
        ClientNodeSerializableParameters clientNodeSerializableParameters;
        public ClientNodeForm(ClientNodeSerializableParameters param) {
            InitializeComponent();
            clientNodeSerializableParameters = param;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            clientNodeSerializableParameters.Id = int.Parse(textBoxId.Text);
            clientNodeSerializableParameters.IpAddress = textBoxIpAddress.Text;
            clientNodeSerializableParameters.ClientName = textBoxClientName.Text;
            clientNodeSerializableParameters.CloudPort = int.Parse(textBoxCloudPort.Text);

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}