using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using NetworkEmulation.editor;
using NetworkEmulation.log;
using NetworkEmulation.Properties;
using NetworkUtilities;
using NetworkEmulation.network;

namespace NetworkEmulation {
    public partial class MainForm : Form {
        private Simulation _simulation;
        public MainForm() {
            InitializeComponent();
        }

        private static Cursor CursorImage(Bitmap b) {
            var resized = new Bitmap(b, new Size(32, 32));
            var ptr = resized.GetHicon();
            return new Cursor(ptr);
        }

        private void newProjectMenuItem_Click(object sender, EventArgs e) {
            CreateNewEditorPanel();
        }

        private EditorPanel CreateNewEditorPanel() {
            Controls.Remove(editorPanel);
            var newEditorPanel = new EditorPanel();
            newEditorPanel.Dock = DockStyle.Fill;
            newEditorPanel.Location = editorPanel.Location;
            newEditorPanel.Size = editorPanel.Size;
            editorPanel = newEditorPanel;
            Controls.Add(editorPanel);

            return editorPanel;
        }

        private void saveProjectMenuItem_Click(object sender, EventArgs e) {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Xml File (.xml)|*.xml";

            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                using (var saveFileDialogStream = saveFileDialog.OpenFile()) {
                    var xml = new XmlDocument();
                    xml.LoadXml(XmlSerializer.Serialize(editorPanel));

                    xml.Save(saveFileDialogStream);
                }
        }

        private void loadProjectMenuItem_Click(object sender, EventArgs e) {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Xml File (.xml)|*.xml";

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                using (var openFileDialogStream = openFileDialog.OpenFile()) {
                    var streamReader = new StreamReader(openFileDialogStream);

                    editorPanel = CreateNewEditorPanel();
                    XmlSerializer.Deserialize(editorPanel, streamReader.ReadToEnd());
                }
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
            _simulation.CableCloudLogForm?.ShowDialog();
        }

        private void networkManagmentSystemToolStripMenuItem_Click(object sender, EventArgs e) {
            _simulation.NetworkManagmentSystemLogForm?.ShowDialog();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e) {
            _simulation = new Simulation(editorPanel._addedNodePictureBoxes, editorPanel._addedLinks, editorPanel._addedConnections);
            MessageBox.Show("Simulation running.");
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e) {
            _simulation?.Stop();
            MessageBox.Show("Simulation stopped.");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            _simulation?.Stop();
            Environment.Exit(0);
        }
    }
}