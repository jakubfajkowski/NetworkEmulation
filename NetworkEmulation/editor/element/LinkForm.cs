using System;
using System.Windows.Forms;
using NetworkEmulation.Network;

namespace NetworkEmulation.Editor.Element {
    public partial class LinkForm : Form {
        private readonly Link _link;

        public LinkForm(Link link) {
            InitializeComponent();
            _link = link;
            FillPortComboBoxes();
        }

        public void AddPortToComboBox(ComboBox cb, int port) {
            cb.Items.Add(port);
        }

        public void FillPortComboBoxes() {
            for (var i = 1; i < 8; i++) {
                AddPortToComboBox(comboBoxInputPort, i);
                AddPortToComboBox(comboBoxOutputPort, i);
            }
        }

        private void OkClick(object sender, EventArgs e) {
            var inputNodePort = int.Parse(comboBoxInputPort.Text);
            var outputNodePort = int.Parse(comboBoxOutputPort.Text);

            var inputSocketPort = _link.BeginNodeView.CableCloudDataPort;
            var outputSocketPort = _link.EndNodeView.CableCloudDataPort;

            _link.Parameters.InputNodePortPair = new SocketNodePortPair(inputNodePort, inputSocketPort);
            _link.Parameters.OutputNodePortPair = new SocketNodePortPair(outputNodePort, outputSocketPort);

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }
    }
}