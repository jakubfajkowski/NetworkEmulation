using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetworkUtilities;
using NetworkUtilities.element;

namespace NetworkEmulation.editor.element {
    public partial class ClientNodeSP : Form {
        public ClientNodeSP() {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            ClientNodeSerializableParameters param = new ClientNodeSerializableParameters {
                Id = int.Parse(textBoxId.Text),
                ClientName = textBoxClientName.Text,
                ClientTable =
                        new List<ClientTableRow>(new[]
                            {new ClientTableRow(textBoxClientName.Text, int.Parse(textBoxClientPort.Text), int.Parse(textBoxVPI.Text), int.Parse(textBoxVCI.Text))}),
                CloudPort = int.Parse(textBoxCloudPort.Text),
                IpAddress = textBoxIpAddress.Text
            };
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
