using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetworkEmulation.network;
using NetworkEmulation.network.element;

namespace NetworkEmulation.editor.element {
    public partial class LinkSP : Form {
        public LinkSP() {
            InitializeComponent();
            ExamplePorts();
        }
        public void AddPortToComboBox(ComboBox cb, int port) {
            cb.Items.Add(port);
        }

        public void ExamplePorts() {
            for (int i = 1; i < 10; i++) {
                AddPortToComboBox(comboInputPort, i);
                AddPortToComboBox(comboOutputPort, i);
            }
        }

        private void OkClick(object sender, EventArgs e) {
            var link = new LinkSerializableParameters() {
                InputNodePortPair = new SocketNodePortPair(Int32.Parse(comboInputPort.Text), 69),
                OutputNodePortPair = new SocketNodePortPair(Int32.Parse(comboOutputPort.Text), 69)
            };
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
