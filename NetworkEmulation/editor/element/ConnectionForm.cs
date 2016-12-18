using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetworkEmulation.network.element;
using NetworkUtilities;
using NetworkUtilities.element;

namespace NetworkEmulation.editor.element {
    public partial class ConnectionForm : Form {
        ConnectionSerializableParameters connectionSP;
        public ConnectionForm(string[] args) {
            
            InitializeComponent();
            var xml = string.Join(" ", args);
            connectionSP = XmlSerializer.Deserialize(xml, typeof(ConnectionSerializableParameters)) as ConnectionSerializableParameters;
            textBoxInputPort.Text = connectionSP;

        }
        private void buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            var link= new LinkSerializableParameters() {
                
            };
            
            this.Close();
        }
    }
}
