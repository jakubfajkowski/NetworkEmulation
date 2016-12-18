using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetworkUtilities;
using NetworkUtilities.element;

namespace NetworkEmulation.editor.element {
    public partial class NetworkNodeSP : Form {
        public NetworkNodeSP() {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            var networkNodeSerializableParameters = new NetworkNodeSerializableParameters {
                Id = int.Parse(textBoxID.Text),
                IpAddress = textBoxIpAddress.Text,
                CloudPort = int.Parse(textBoxCloudPort.Text),
                NetworkManagmentSystemPort = int.Parse(textBoxNMSPort.Text),
                NumberOfPorts = int.Parse(textBoxNumberOfPorts.Text)
            };
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
