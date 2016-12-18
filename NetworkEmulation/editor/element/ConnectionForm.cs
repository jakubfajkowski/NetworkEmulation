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

namespace NetworkEmulation.editor.element {
    public partial class ConnectionForm : Form {
        public ConnectionForm() {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            var link= new LinkSerializableParameters() {
                
            };

            new ConnectionSerializableParameters() {
                
            }
            this.Close();
        }
    }
}
