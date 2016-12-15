using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NetworkEmulation.editor {
    public enum Mode {
        AddClientNode,
        AddNetworkNode,
        AddLink,
        AddConnection,
        Move,
        Delete
    }

    public class EditorPanel : Panel {
        private readonly List<Connection> _addedConnections = new List<Connection>();
        private readonly List<Link> _addedLinks = new List<Link>();
        private readonly List<NodePictureBox> _addedNodePictureBoxes = new List<NodePictureBox>();
        private List<Link> _currentConnectionLinks;
        private NodePictureBox _currentNodePictureBox;

        private bool _handlingAddingConnection;
        private Mode _mode = Mode.Move;

        public EditorPanel() {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);
        }

        public Mode Mode {
            private get { return _mode; }
            set {
                _mode = value;
                Deselect(_currentNodePictureBox);
                _currentNodePictureBox = null;
                _currentConnectionLinks = new List<Link>();
                _handlingAddingConnection = false;
            }
        }

        private NodePictureBox CurrentNodePictureBox {
            get { return _currentNodePictureBox; }
            set {
                Deselect(_currentNodePictureBox);

                _currentNodePictureBox = value;

                Select(_currentNodePictureBox);
            }
        }

        private bool HandlingAddingConnection {
            get { return _handlingAddingConnection; }
            set {
                _handlingAddingConnection = value;
                if (_handlingAddingConnection) _currentConnectionLinks = new List<Link>();
                else _currentConnectionLinks = null;
            }
        }

        private void Select(IMarkable markable) {
            markable?.MarkAsSelected();
        }

        private void Deselect(IMarkable markable) {
            markable?.MarkAsDeselected();
        }

        private void nodePictureBox_Click(object sender, EventArgs e) {
            if (Mode == Mode.AddConnection)
                if (!HandlingAddingConnection) {
                    BeginHandlingAddingConnection(sender as NodePictureBox);
                }
                else {
                    var previousNodePictureBox = CurrentNodePictureBox;
                    CurrentNodePictureBox = sender as NodePictureBox;
                    HandleAddingConnection(previousNodePictureBox, CurrentNodePictureBox);
                }
            if (Mode == Mode.AddLink)
                if (CurrentNodePictureBox == null) {
                    CurrentNodePictureBox = sender as NodePictureBox;
                }
                else {
                    AddLinkPictureBox(CurrentNodePictureBox, sender as NodePictureBox);
                    CurrentNodePictureBox = null;
                }
            if (Mode == Mode.Delete)
                MessageBox.Show("Not implemented."); //DeleteNodePictureBox(sender as NodePictureBox);
        }

        private void BeginHandlingAddingConnection(NodePictureBox beginNodePictureBox) {
            if (beginNodePictureBox is NetworkNodePictureBox)
                FailHandlingAddingConnection("Connection should begin with client node.");
            if (beginNodePictureBox is ClientNodePictureBox) {
                HandlingAddingConnection = true;
                CurrentNodePictureBox = beginNodePictureBox;
            }
        }

        private void HandleAddingConnection(NodePictureBox previousNodePictureBox, NodePictureBox nextNodePictureBox) {
            if (nextNodePictureBox is ClientNodePictureBox) {
                var link = FindLinksBetweenNodes(previousNodePictureBox, nextNodePictureBox);

                if (link != null) {
                    AddLinkToCurrentConnection(link);
                    EndHandlingAddingConnection(_currentConnectionLinks);
                }
                else {
                    FailHandlingAddingConnection("There is no link between specified nodes.");
                }
            }
            if (nextNodePictureBox is NetworkNodePictureBox) {
                var link = FindLinksBetweenNodes(previousNodePictureBox, nextNodePictureBox);

                if (link != null) AddLinkToCurrentConnection(link);
                else FailHandlingAddingConnection("There is no link between specified nodes.");
            }
        }

        private Link FindLinksBetweenNodes(NodePictureBox beginNodePictureBox, NodePictureBox endNodePictureBox) {
            foreach (var link in _addedLinks)
                if (link.IsBetween(beginNodePictureBox, endNodePictureBox))
                    return link;

            return null;
        }

        private void AddLinkToCurrentConnection(Link link) {
            _currentConnectionLinks.Add(link);
        }

        private void EndHandlingAddingConnection(List<Link> currentConnectionLinks) {
            AddConnection(currentConnectionLinks);
            _handlingAddingConnection = false;
        }

        private void FailHandlingAddingConnection(string message) {
            MessageBox.Show(message, "Add Connection Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _handlingAddingConnection = false;
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            switch (Mode) {
                case Mode.AddClientNode:
                    _currentNodePictureBox = new ClientNodePictureBox();
                    break;

                case Mode.AddNetworkNode:
                    _currentNodePictureBox = new NetworkNodePictureBox();
                    break;

                default:
                    return;
            }

            AddNodePictureBox(_currentNodePictureBox);
            _currentNodePictureBox.Location = e.Location;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            var graphics = e.Graphics;
            foreach (var insertedLink in _addedLinks) insertedLink.DrawLink(graphics);
        }

        private void AddNodePictureBox(NodePictureBox nodePictureBox) {
            nodePictureBox.Click += nodePictureBox_Click;
            Controls.Add(nodePictureBox);
            _addedNodePictureBoxes.Add(nodePictureBox);
        }

        private void DeleteNodePictureBox(NodePictureBox nodePictureBox) {
            Controls.Remove(nodePictureBox);
            _addedNodePictureBoxes.Remove(nodePictureBox);
        }

        private void AddLinkPictureBox(NodePictureBox beginNodePictureBox, NodePictureBox endNodePictureBox) {
            var link = new Link(ref beginNodePictureBox, ref endNodePictureBox);
            Controls.Add(link);
            _addedLinks.Add(link);
            Refresh();
        }

        private void AddConnection(List<Link> connectionLinks) {
            _addedConnections.Add(new Connection(connectionLinks));
        }

        //}
        //    return Activator.CreateInstance(obj.GetType());

        //private static object NewInstance(object obj) {
    }
}