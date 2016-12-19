using System;
using System.Windows.Forms;
using NetworkUtilities;
using NetworkUtilities.element;

namespace NetworkEmulation.editor.element {
    public partial class ClientNodeForm : Form {
        private readonly ClientNodeSerializableParameters clientNodeSerializableParameters;

        public ClientNodeForm(ClientNodeSerializableParameters param) {
            InitializeComponent();
            clientNodeSerializableParameters = param;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            clientNodeSerializableParameters.IpAddress = textBoxIpAddress.Text;
            clientNodeSerializableParameters.ClientName = textBoxClientName.Text;
            clientNodeSerializableParameters.CloudPort = int.Parse(textBoxCloudPort.Text);
            clientNodeSerializableParameters.ClientTable.Add(new ClientTableRow(textBoxClientName.Text,
                int.Parse(textBoxClientPort.Text), int.Parse(textBoxVPI.Text), int.Parse(textBoxVCI.Text)));

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }
    }
}