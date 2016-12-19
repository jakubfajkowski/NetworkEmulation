using System;
using System.Windows.Forms;

namespace NetworkEmulation.editor.element {
    public partial class ConnectionForm : Form {
        public ConnectionForm() {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            Close();
        }
    }
}