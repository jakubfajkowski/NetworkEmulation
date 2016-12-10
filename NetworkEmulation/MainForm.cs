using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NetworkEmulation.Properties;

namespace NetworkEmulation {
    public enum EditorMode { AddNode, AddLink, Move, Delete }

    public partial class MainForm : Form {
        private EditorMode _editorMode = EditorMode.Move;
        private NodePictureBox _selectedNodePictureBox;
        private List<NodePictureBox> _insertedNodePictureBoxes = new List<NodePictureBox>();

        public MainForm() {
            InitializeComponent();
        }

        private void editorPanel_MouseClick(object sender, MouseEventArgs e) {
            if (_editorMode != EditorMode.AddNode) return;

            _selectedNodePictureBox.Location = e.Location;
            AddNodePictureBox(_selectedNodePictureBox);
            _selectedNodePictureBox = NewInstance(_selectedNodePictureBox) as NodePictureBox;
        }

        private void nodePictureBox_Click(object sender, EventArgs e) {
            if (_editorMode == EditorMode.Delete) 
                DeleteNodePictureBox(sender as NodePictureBox);
            if (_editorMode == EditorMode.AddLink)
                if (_selectedNodePictureBox == null) {
                    _selectedNodePictureBox = sender as NodePictureBox;
                }
                else {
                    AddLinkPictureBox(_selectedNodePictureBox, sender as NodePictureBox);
                    _selectedNodePictureBox = null;
                }
        }

        private void AddNodePictureBox(NodePictureBox nodePictureBox) {
            nodePictureBox.Click += nodePictureBox_Click;
            editorPanel.Controls.Add(nodePictureBox);
            _insertedNodePictureBoxes.Add(nodePictureBox);
        }

        private void DeleteNodePictureBox(NodePictureBox nodePictureBox) {
            editorPanel.Controls.Remove(nodePictureBox);
            _insertedNodePictureBoxes.Remove(nodePictureBox);
        }

        private void AddLinkPictureBox(NodePictureBox beginNodePictureBox, NodePictureBox endNodePictureBox) {
            var linkPictureBox = new LinkPictureBox(beginNodePictureBox, endNodePictureBox);
            editorPanel.Controls.Add(linkPictureBox);
        }

        private static object NewInstance(object obj) {
            return Activator.CreateInstance(obj.GetType());
        }

        private void clientNodeToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = CursorImage(Resources.ClientNodeNotSelected);
            _selectedNodePictureBox = new ClientNodePictureBox();
            _editorMode = EditorMode.AddNode;
        }

        private void networkNodeToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = CursorImage(Resources.NetworkNodeNotSelected);
            _selectedNodePictureBox = new NetworkNodePictureBox();
            _editorMode = EditorMode.AddNode;
        }

        private static Cursor CursorImage(Bitmap b) {
            var resized = new Bitmap(b, new Size(32, 32));
            var ptr = resized.GetHicon();
            return new Cursor(ptr);
        }

        private void linkToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = Cursors.SizeAll;
            _editorMode = EditorMode.AddLink;

            _selectedNodePictureBox = null;
        }

        private void connectionToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = Cursors.Hand;
            _editorMode = EditorMode.Move;

            _selectedNodePictureBox = null;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = CursorImage(Resources.Delete);
            _editorMode = EditorMode.Delete;
            
            _selectedNodePictureBox = null;
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
