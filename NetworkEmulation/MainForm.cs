using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using NetworkEmulation.Editor;
using NetworkEmulation.Editor.Element;
using NetworkEmulation.Network;
using NetworkEmulation.Properties;
using NetworkUtilities;
using NetworkUtilities.Log;
using NetworkUtilities.Serialization;

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
            editorPanel.Clear();
            networkHierarchyTreeView.Nodes.Clear();
            parametersListView.Items.Clear();
        }

        private void editorPanel_ControlAdded(object sender, ControlEventArgs e) {
            if (e.Control is ClientNode) {
                var clientNode = (ClientNode) e.Control;
                AddToNetworkHierarchyTreeView(clientNode);
            }

            if (e.Control is NetworkNode) {
                var networkNode = (NetworkNode) e.Control;
                AddToNetworkHierarchyTreeView(networkNode);
            }
        }

        private void AddToNetworkHierarchyTreeView(ClientNode node) {
            var treeNode = new TreeNode {
                Tag = node
            };

            var address = AddNodeToNetworkHierachryTreeView(treeNode);
            node.Parameters.NetworkAddress = address;
            treeNode.Text = address.ToString();
        }


        private void AddToNetworkHierarchyTreeView(NetworkNode node) {
            var treeNode = new TreeNode {
                Tag = node
            };

            var address = AddNodeToNetworkHierachryTreeView(treeNode);
            node.Parameters.NetworkAddress = address;
            treeNode.Text = address.ToString();
        }

        private NetworkAddress AddNodeToNetworkHierachryTreeView(TreeNode treeNode) {
            NetworkAddress nodeAddress;
            var selectedNode = networkHierarchyTreeView.SelectedNode;

            if (selectedNode != null) {
                selectedNode.Nodes.Add(treeNode);

                var parentAddress = new NetworkAddress(selectedNode.Text);
                var childrenIndex = selectedNode.Nodes.IndexOf(treeNode) + 1;
                nodeAddress = parentAddress.Append(childrenIndex);
            }
            else {
                networkHierarchyTreeView.Nodes.Add(treeNode);

                var childrenIndex = networkHierarchyTreeView.Nodes.IndexOf(treeNode) + 1;
                nodeAddress = new NetworkAddress(childrenIndex);
            }

            return nodeAddress;
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

                    editorPanel.Clear();
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

        private void moveToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = Cursors.Hand;
            editorPanel.Mode = Mode.Move;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = CursorImage(Resources.Delete);
            editorPanel.Mode = Mode.Delete;
        }

        private void cableCloudToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_simulation == null) return;
            HandleLogFormOpen(_simulation.CableCloudLogForm);
        }

        private void networkManagmentSystemToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_simulation == null) return;
            HandleLogFormOpen(_simulation.NetworkManagmentSystemLogForm);
        }

        private void HandleLogFormOpen(LogForm logForm) {
            if (!logForm.Visible)
                logForm.Show();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e) {
            _simulation = new Simulation(editorPanel.AddedNodePictureBoxes, editorPanel.AddedLinks);
            MessageBox.Show("Simulation running.");
            SwitchRunStop();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e) {
            _simulation?.Stop();
            MessageBox.Show("Simulation stopped.");
            SwitchRunStop();
        }

        private void SwitchRunStop() {
            runToolStripMenuItem.Enabled = !runToolStripMenuItem.Enabled;
            stopToolStripMenuItem.Enabled = !runToolStripMenuItem.Enabled;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            _simulation?.Stop();
            Environment.Exit(0);
        }
    }
}