using System;
using System.Windows.Forms;
using NetworkEmulation.network;

namespace NetworkEmulation.editor.element {
    public partial class ConnectionForm : Form {
        private readonly Connection _connection;

        public ConnectionForm(Connection connection) {
            InitializeComponent();
            _connection = connection;
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            _connection.Parameters.NodeConnectionInformations.Add(new NodeConnectionInformation {
                InVpi = int.Parse(textBoxInputVpi.Text),
                InVci = int.Parse(textBoxInputVci.Text),
                InPortNumber = int.Parse(textBoxInputPort.Text),
                OutVpi = int.Parse(textBoxOutputVpi.Text),
                OutVci = int.Parse(textBoxOutputVci.Text),
                OutPortNumber = int.Parse(textBoxOutputPort.Text),
            });

            Close();
        }
    }
}