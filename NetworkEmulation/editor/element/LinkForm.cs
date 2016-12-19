using System;
using System.Windows.Forms;
using NetworkEmulation.network;
using NetworkEmulation.network.element;

namespace NetworkEmulation.editor.element {
    public partial class LinkForm : Form {
        public LinkForm() {
            InitializeComponent();
            ExamplePorts();
        }

        public void AddPortToComboBox(ComboBox cb, int port) {
            cb.Items.Add(port);
        }

        public void ExamplePorts() {
            for (var i = 1; i < 10; i++) {
                AddPortToComboBox(comboInputPort, i);
                AddPortToComboBox(comboOutputPort, i);
            }
        }

        private void OkClick(object sender, EventArgs e) {
            var link = new LinkSerializableParameters {
                InputNodePortPair = new SocketNodePortPair(int.Parse(comboInputPort.Text), 69),
                OutputNodePortPair = new SocketNodePortPair(int.Parse(comboOutputPort.Text), 69)
            };
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }
    }
}