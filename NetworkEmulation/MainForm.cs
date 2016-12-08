using System;
using System.Drawing;
using System.Windows.Forms;
using NetworkEmulation.Properties;

namespace NetworkEmulation {
    public partial class MainForm : Form {
        private NodePictureBox selectedNodePictureBox;

        public MainForm() {
            InitializeComponent();
        }

        private void editorPanel_MouseClick(object sender, MouseEventArgs e) {
            if (selectedNodePictureBox != null) {
                selectedNodePictureBox.Location = e.Location;
                editorPanel.Controls.Add(selectedNodePictureBox);
                selectedNodePictureBox = null;
            }
        }

        private void clientNodeToolStripMenuItem_Click(object sender, EventArgs e) {
            changeCursorToNodeImage(Resources.ClientNodeNotSelected);
            selectedNodePictureBox = new ClientNodePictureBox();
        }

        private void networkNodeToolStripMenuItem_Click(object sender, EventArgs e) {
            changeCursorToNodeImage(Resources.NetworkNodeNotSelected);
            selectedNodePictureBox = new NetworkNodePictureBox();
        }

        private void changeCursorToNodeImage(Bitmap b) {
            IntPtr ptr = b.GetHicon();
            Cursor c = new Cursor(ptr);
            this.editorPanel.Cursor = c;
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e) {
            this.editorPanel.Cursor = Cursors.Hand;

            selectedNodePictureBox = null;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            this.editorPanel.Cursor = Cursors.Cross;

            selectedNodePictureBox = null;
        }
    }
}
