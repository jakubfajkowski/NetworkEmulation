using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetworkEmulation.Properties;

namespace NetworkEmulation {
    public enum Mode {
        AddClientNode,
        AddNetworkNode,
        AddLink,
        Move,
        Delete
    }

    public class EditorPanel : Panel {
        private Mode _mode;

        public Mode Mode {
            private get { return _mode; }
            set {
                _mode = value;
                DeselectNodePictureBox(ref _currentNodePictureBox);
                _currentNodePictureBox = null;
            }
        }

        private readonly List<Link> _insertedLinks = new List<Link>();
        private readonly List<NodePictureBox> _insertedNodePictureBoxes = new List<NodePictureBox>();
        private NodePictureBox _currentNodePictureBox;

        private NodePictureBox CurrentNodePictureBox {
            get { return _currentNodePictureBox; }
            set {
                DeselectNodePictureBox(ref _currentNodePictureBox);

                _currentNodePictureBox = value;

                SelectNodePictureBox(ref _currentNodePictureBox);
            }
        }

        public EditorPanel() {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);
        }

        private void SelectNodePictureBox(ref NodePictureBox nodePictureBox) {
            if (nodePictureBox == null) return;

            if (nodePictureBox is ClientNodePictureBox)
                nodePictureBox.Image = Resources.ClientNodeSelected;
            if (nodePictureBox is NetworkNodePictureBox)
                nodePictureBox.Image = Resources.NetworkNodeSelected;
        }

        private void DeselectNodePictureBox(ref NodePictureBox nodePictureBox) {
            if (nodePictureBox == null) return;

            if (nodePictureBox is ClientNodePictureBox)
                nodePictureBox.Image = Resources.ClientNodeNotSelected;
            if (nodePictureBox is NetworkNodePictureBox)
                nodePictureBox.Image = Resources.NetworkNodeNotSelected;
        }

        private void AddNodePictureBox(NodePictureBox nodePictureBox) {
            nodePictureBox.Click += nodePictureBox_Click;
            Controls.Add(nodePictureBox);
            _insertedNodePictureBoxes.Add(nodePictureBox);
        }

        private void DeleteNodePictureBox(NodePictureBox nodePictureBox) {
            Controls.Remove(nodePictureBox);
            _insertedNodePictureBoxes.Remove(nodePictureBox);
        }

        private void nodePictureBox_Click(object sender, EventArgs e) {
            if (Mode == Mode.Delete)
                DeleteNodePictureBox(sender as NodePictureBox);
            if (Mode == Mode.AddLink)
                if (CurrentNodePictureBox == null) {
                    CurrentNodePictureBox = sender as NodePictureBox;
                }
                else {
                    AddLinkPictureBox(CurrentNodePictureBox, sender as NodePictureBox);
                    CurrentNodePictureBox = null;
                }
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
            foreach (var insertedLink in _insertedLinks) insertedLink.DrawLink(graphics);
        }

        private void AddLinkPictureBox(NodePictureBox beginNodePictureBox, NodePictureBox endNodePictureBox) {
            var link = new Link(ref beginNodePictureBox, ref endNodePictureBox);
            Controls.Add(link);
            _insertedLinks.Add(link);
            Refresh();
        }

        //private static object NewInstance(object obj) {
        //    return Activator.CreateInstance(obj.GetType());
        //}
    }
}