using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NetworkEmulation.Editor.Element;

namespace NetworkEmulation.Editor {
    public enum Mode {
        AddClientNode,
        AddNetworkNode,
        AddLink,
        Move,
        Delete
    }

    [Serializable]
    public partial class EditorPanel : UserControl, IXmlSerializable {
        private NodePictureBox _currentNodePictureBox;

        private Mode _mode = Mode.Move;

        public EditorPanel() {
            InitializeComponent();
        }
        
        public List<Link> AddedLinks { get; } = new List<Link>();
        public List<NodePictureBox> AddedNodePictureBoxes { get; } = new List<NodePictureBox>();

        private NodePictureBox CurrentNodePictureBox {
            get { return _currentNodePictureBox; }
            set {
                Deselect(_currentNodePictureBox);

                _currentNodePictureBox = value;

                Select(_currentNodePictureBox);
            }
        }

        public Mode Mode {
            private get { return _mode; }
            set {
                _mode = value;
                Deselect(_currentNodePictureBox);
                _currentNodePictureBox = null;
            }
        }

        private void Select(IMarkable markable) {
            markable?.MarkAsSelected();
        }

        private void Deselect(IMarkable markable) {
            markable?.MarkAsDeselected();
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            switch (Mode) {
                case Mode.AddClientNode:
                    var clientNodePictureBox = new ClientNodePictureBox();
                    _currentNodePictureBox = clientNodePictureBox;
                    new ClientNodeForm(clientNodePictureBox).ShowDialog(this);
                    break;

                case Mode.AddNetworkNode:
                    var networkNodePictureBox = new NetworkNodePictureBox();
                    _currentNodePictureBox = networkNodePictureBox;
                    new NetworkNodeForm(networkNodePictureBox).ShowDialog(this);
                    break;

                default:
                    return;
            }

            Add(_currentNodePictureBox);
            _currentNodePictureBox.Location = e.Location;
        }

        private void nodePictureBox_OnClick(object sender, EventArgs e) {
            switch (Mode) {
                case Mode.AddLink:
                    if (CurrentNodePictureBox == null) {
                        CurrentNodePictureBox = sender as NodePictureBox;
                    }
                    else {
                        var link = CreateLink(CurrentNodePictureBox, sender as NodePictureBox);
                        new LinkForm(link).ShowDialog(this);
                        Add(link);
                        link.MarkAsDeselected();
                        CurrentNodePictureBox = null;
                    }
                    break;

                case Mode.Delete:
                    MessageBox.Show("Not implemented.");
                    break;

                default:
                    return;
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            var graphics = e.Graphics;
            foreach (var insertedLink in AddedLinks) insertedLink.DrawLink(graphics);
            foreach (var addedNodePictureBox in AddedNodePictureBoxes) {
                var clientNodePictureBox = addedNodePictureBox as ClientNodePictureBox;

                if (clientNodePictureBox == null) continue;
                using (var myFont = new Font("Arial", 8)) {
                    var centerPoint = clientNodePictureBox.CenterPoint();
                    var namePoint = new Point(centerPoint.X - 12, centerPoint.Y + 32);
                    graphics.DrawString(clientNodePictureBox.Parameters.ClientName, myFont, Brushes.Black, namePoint);
                }
            }
        }

        private void Add(NodePictureBox nodePictureBox) {
            nodePictureBox.Click += nodePictureBox_OnClick;
            Controls.Add(nodePictureBox);
            AddedNodePictureBoxes.Add(nodePictureBox);

            Deselect(nodePictureBox);
        }

        private void DeleteNodePictureBox(NodePictureBox nodePictureBox) {
            Controls.Remove(nodePictureBox);
            AddedNodePictureBoxes.Remove(nodePictureBox);
        }

        private void Add(Link link) {
            Controls.Add(link);
            AddedLinks.Add(link);

            Deselect(link);
        }

        private Link CreateLink(NodePictureBox beginNodePictureBox, NodePictureBox endNodePictureBox) {
            return new Link(ref beginNodePictureBox, ref endNodePictureBox);
        }

        #region IXmlSerializable

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            var clientNodePictureBoxSerializer = new XmlSerializer(typeof(List<ClientNodePictureBox>));
            var networkNodePictureBoxSerializer = new XmlSerializer(typeof(List<NetworkNodePictureBox>));
            var linkSerializer = new XmlSerializer(typeof(List<Link>));

            reader.ReadStartElement(nameof(EditorPanel));
            var clientNodePictureBoxes =
                clientNodePictureBoxSerializer.Deserialize(reader) as List<ClientNodePictureBox>;
            if (clientNodePictureBoxes != null)
                foreach (var clientNodePictureBox in clientNodePictureBoxes) Add(clientNodePictureBox);

            var networkNodePictureBoxes =
                networkNodePictureBoxSerializer.Deserialize(reader) as List<NetworkNodePictureBox>;
            if (networkNodePictureBoxes != null)
                foreach (var networkNodePictureBox in networkNodePictureBoxes) Add(networkNodePictureBox);

            foreach (var link in linkSerializer.Deserialize(reader) as List<Link>) {
                RestoreReferences(link);
                Add(link);
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            var clientNodePictureBoxSerializer = new XmlSerializer(typeof(List<ClientNodePictureBox>));
            var networkNodePictureBoxSerializer = new XmlSerializer(typeof(List<NetworkNodePictureBox>));
            var linkSerializer = new XmlSerializer(typeof(List<Link>));

            clientNodePictureBoxSerializer.Serialize(writer,
                AddedNodePictureBoxes.OfType<ClientNodePictureBox>().ToList());
            networkNodePictureBoxSerializer.Serialize(writer,
                AddedNodePictureBoxes.OfType<NetworkNodePictureBox>().ToList());
            linkSerializer.Serialize(writer, AddedLinks);
        }

        private void RestoreReferences(Link link) {
            var beginNodePictureBoxId = link.Parameters.BeginNodePictureBoxId;
            var endNodePictureBoxId = link.Parameters.EndNodePictureBoxId;

            var beginNodePictureBox = AddedNodePictureBoxes.Find(box => box.Id.Equals(beginNodePictureBoxId));
            var endNodePictureBox = AddedNodePictureBoxes.Find(box => box.Id.Equals(endNodePictureBoxId));

            link.SetAttachmentNodePictureBoxes(ref beginNodePictureBox, ref endNodePictureBox);
        }

        #endregion
    }
}