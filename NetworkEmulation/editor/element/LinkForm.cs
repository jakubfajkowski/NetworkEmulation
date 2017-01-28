using System;
using System.Windows.Forms;
using NetworkEmulation.Network;

namespace NetworkEmulation.Editor.Element {
    public partial class LinkForm : Form {
        private readonly LinkView _link;

        public LinkForm(LinkView link) {
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

            var inputNetworkAddress = _link.BeginNodeView.NetworkAddress;
            var outputNetworkAddress = _link.EndNodeView.NetworkAddress;

            _link.Parameters.InputNodePortPair = new NetworkAddressNodePortPair(inputNetworkAddress, inputNodePort);
            _link.Parameters.OutputNodePortPair = new NetworkAddressNodePortPair(outputNetworkAddress, outputNodePort);

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }
    }
}