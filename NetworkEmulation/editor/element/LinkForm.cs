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
using NetworkUtilities;

namespace NetworkEmulation.editor.element {
    public partial class LinkForm : Form {
        private LinkSerializableParameters linkSP;
        public LinkForm(string[] args) {
            var xml = string.Join(" ", args);
            linkSP = XmlSerializer.Deserialize(xml, typeof(LinkSerializableParameters)) as LinkSerializableParameters;
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
            linkSP.InputNodePortPair.NodePortNumber = int.Parse(comboInputPort.Text);
            linkSP.OutputNodePortPair.NodePortNumber = int.Parse(comboOutputPort.Text);

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
