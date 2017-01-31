using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using NetworkEmulation.Editor;
using NetworkEmulation.Editor.Element;
using NetworkEmulation.Properties;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.Log;
using NetworkUtilities.Utilities;
using NetworkUtilities.Utilities.Serialization;

namespace NetworkEmulation {
    public partial class MainForm : Form {
        private Simulation _simulation;

        public MainForm() {
            InitializeComponent();
            NewProject();
        }

        private static Cursor CursorImage(Bitmap b) {
            var resized = new Bitmap(b, new Size(32, 32));
            var ptr = resized.GetHicon();
            return new Cursor(ptr);
        }

        private void newProjectMenuItem_Click(object sender, EventArgs e) {
            NewProject();
        }

        private void NewProject() {
            editorPanel.Clear();
            networkHierarchyTreeView.Nodes.Clear();
            AddTopTwoDomains();
            parametersListView.Items.Clear();
        }

        private void AddTopTwoDomains() {
            AddSubnetworkNode();
            networkHierarchyTreeView.Nodes[0].Tag = new StepByStepPathComputationServer(new NetworkAddress(1),
                "127.0.0.1", Settings.Default.SignallingCloudListeningPort);
            AddSubnetworkNode();
            networkHierarchyTreeView.Nodes[1].Tag = new StepByStepPathComputationServer(new NetworkAddress(2),
                "127.0.0.1", Settings.Default.SignallingCloudListeningPort);

            networkHierarchyTreeView.SelectedNode = networkHierarchyTreeView.Nodes[0];
        }

        private void editorPanel_ControlAdded(object sender, ControlEventArgs e) {
            if (e.Control is ClientNodeView) {
                var clientNode = (ClientNodeView) e.Control;

                if (clientNode.Parameters.NetworkAddress == null)
                    AddToNetworkHierarchyTreeView(clientNode);
                else
                    RestoreToNetworkHierarchyTreeView(clientNode.Parameters.NetworkAddress, clientNode);
            }

            if (e.Control is NetworkNodeView) {
                var networkNode = (NetworkNodeView) e.Control;

                if (networkNode.Parameters.NetworkAddress == null)
                    AddToNetworkHierarchyTreeView(networkNode);
                else
                    RestoreToNetworkHierarchyTreeView(networkNode.Parameters.NetworkAddress, networkNode);
            }
        }

        private void AddToNetworkHierarchyTreeView(ClientNodeView node) {
            var treeNode = new TreeNode {
                Tag = node
            };

            var address = AddToTreeView(treeNode);
            node.Parameters.NetworkAddress = address;
            node.NetworkAddress = address;
            treeNode.Text = address.ToString();
        }

        private void AddToNetworkHierarchyTreeView(NetworkNodeView node) {
            var treeNode = new TreeNode {
                Tag = node
            };

            var address = AddToTreeView(treeNode);
            node.Parameters.NetworkAddress = address;
            node.NetworkAddress = address;
            treeNode.Text = address.ToString();
        }

        private void RestoreToNetworkHierarchyTreeView(NetworkAddress address, NodeView node) {
            var nodes = networkHierarchyTreeView.Nodes;
            TreeNode parent = null;
            var currentAddress = new NetworkAddress(address.GetId(0));

            for (var i = 0; i < address.Levels - 1; i++) {
                var id = address.GetId(i);

                if (nodes.Count < id)
                    while (nodes.Count < id) {
                        var subnetworkNode = new TreeNode();
                        nodes.Add(subnetworkNode);
                        currentAddress = currentAddress.GetParentsAddress().Append(nodes.Count);
                        subnetworkNode.Text = currentAddress.ToString();
                        subnetworkNode.Tag = new HierarchicalPathComputationServer(currentAddress,
                            "127.0.0.1", Settings.Default.SignallingCloudListeningPort);
                    }

                parent = nodes[id - 1];
                nodes = parent.Nodes;
                currentAddress = currentAddress.Append(parent.Index + 1);
            }

            var treeNode = new TreeNode {
                Tag = node
            };
            treeNode.Text = address.ToString();
            parent.Nodes.Add(treeNode);
        }

        private NetworkAddress AddToTreeView(TreeNode treeNode) {
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

        private void networkHierarchyTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
            if (e.Node == null) {
                networkHierarchyTreeView.SelectedNode = networkHierarchyTreeView.TopNode;
                return;
            }

            if (e.Node.Tag is NodeView) networkHierarchyTreeView.SelectedNode = e.Node.Parent;
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

                    NewProject();
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

        private void subnetworkToolStripMenuItem_Click(object sender, EventArgs e) {
            AddSubnetworkNode();
        }

        private void AddSubnetworkNode() {
            var subnetworkNode = new TreeNode();

            if (networkHierarchyTreeView.SelectedNode != null) {
                networkHierarchyTreeView.SelectedNode.Nodes.Add(subnetworkNode);
                var parentAddress = networkHierarchyTreeView.SelectedNode.Text;
                var childIndex = networkHierarchyTreeView.SelectedNode.Nodes.IndexOf(subnetworkNode) + 1;
                var childAddress = new NetworkAddress(parentAddress).Append(childIndex);
                subnetworkNode.Text = childAddress.ToString();
            }
            else {
                networkHierarchyTreeView.Nodes.Add(subnetworkNode);
                var childIndex = networkHierarchyTreeView.Nodes.IndexOf(subnetworkNode) + 1;
                var childAddress = new NetworkAddress(childIndex);
                subnetworkNode.Text = childAddress.ToString();
            }


            subnetworkNode.Tag = new HierarchicalPathComputationServer(new NetworkAddress(subnetworkNode.Text),
                             "127.0.0.1", Settings.Default.SignallingCloudListeningPort);
        }

        private void moveToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = Cursors.Hand;
            editorPanel.Mode = Mode.Move;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            editorPanel.Cursor = CursorImage(Resources.Delete);
            editorPanel.Mode = Mode.Delete;
        }

        private void connectionManagersToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_simulation == null) return;
            HandleLogFormOpen(_simulation.ConnectionManagersLogForm);
        }

        private void pathComputationServersToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_simulation == null) return;
            HandleLogFormOpen(_simulation.PathComputationServersLogForm);
        }

        private void nameServerToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_simulation == null) return;
            HandleLogFormOpen(_simulation.NameServerLogForm);
        }

        private void HandleLogFormOpen(Form logForm) {
            if (!logForm.Visible)
                logForm.Show();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e) {
            _simulation = new Simulation(networkHierarchyTreeView.Nodes, editorPanel.AddedLinks);
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