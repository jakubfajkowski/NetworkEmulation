using System;
using System.Drawing;
using System.Windows.Forms;
using NetworkEmulation.editor;
using NetworkEmulation.Properties;

namespace NetworkEmulation {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private static Cursor CursorImage(Bitmap b) {
            var resized = new Bitmap(b, new Size(32, 32));
            var ptr = resized.GetHicon();
            return new Cursor(ptr);
        }


        private void clientNodeToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = CursorImage(Resources.ClientNodeNotSelected);
            editorPanel.Mode = Mode.AddClientNode;
        }

        private void networkNodeToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = CursorImage(Resources.NetworkNodeNotSelected);
            editorPanel.Mode = Mode.AddNetworkNode;
        }

        private void linkToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = Cursors.SizeAll;
            editorPanel.Mode = Mode.AddLink;
        }

        private void connectionToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = Cursors.Cross;
            editorPanel.Mode = Mode.AddConnection;
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = Cursors.Hand;
            editorPanel.Mode = Mode.Move;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = CursorImage(Resources.Delete);
            editorPanel.Mode = Mode.Delete;
        }

        private void cableCloudToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        private void networkManagmentSystemToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e) {
        }
    }
}