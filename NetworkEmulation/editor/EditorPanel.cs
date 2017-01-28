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
        private NodeView _selectedNodeView;

        private Mode _mode = Mode.Move;

        public EditorPanel() {
            InitializeComponent();
        }
        
        public List<Link> AddedLinks { get; } = new List<Link>();
        public List<NodeView> AddedNodeViews { get; } = new List<NodeView>();

        private NodeView SelectedNodeView {
            get { return _selectedNodeView; }
            set {
                Deselect(_selectedNodeView);

                _selectedNodeView = value;

                Select(_selectedNodeView);
            }
        }

        public Mode Mode {
            private get { return _mode; }
            set {
                _mode = value;
                Deselect(_selectedNodeView);
                _selectedNodeView = null;
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
                    var clientNodeView = new ClientNodeView();
                    _selectedNodeView = clientNodeView;
                    new ClientNodeForm(clientNodeView).ShowDialog(this);
                    break;

                case Mode.AddNetworkNode:
                    var networkNodeView = new NetworkNodeView();
                    _selectedNodeView = networkNodeView;
                    new NetworkNodeForm(networkNodeView).ShowDialog(this);
                    break;

                default:
                    return;
            }

            Add(_selectedNodeView);
            _selectedNodeView.Location = e.Location;
        }

        private void nodeView_OnClick(object sender, EventArgs e) {
            switch (Mode) {
                case Mode.AddLink:
                    if (SelectedNodeView == null) {
                        SelectedNodeView = sender as NodeView;
                    }
                    else {
                        var link = CreateLink(SelectedNodeView, sender as NodeView);
                        new LinkForm(link).ShowDialog(this);
                        Add(link);
                        link.MarkAsDeselected();
                        SelectedNodeView = null;
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
            foreach (var addedNodeView in AddedNodeViews) {
                var clientNodeView = addedNodeView as ClientNodeView;

                if (clientNodeView == null) continue;
                using (var myFont = new Font("Arial", 8)) {
                    var centerPoint = clientNodeView.CenterPoint();
                    var namePoint = new Point(centerPoint.X - 12, centerPoint.Y + 32);
                    graphics.DrawString(clientNodeView.Parameters.ClientName, myFont, Brushes.Black, namePoint);
                }
            }
        }

        private void Add(NodeView nodeView) {
            nodeView.Click += nodeView_OnClick;
            Controls.Add(nodeView);
            AddedNodeViews.Add(nodeView);

            Deselect(nodeView);
        }

        private void DeleteNodeView(NodeView nodeView) {
            Controls.Remove(nodeView);
            AddedNodeViews.Remove(nodeView);
        }

        private void Add(Link link) {
            Controls.Add(link);
            AddedLinks.Add(link);

            Deselect(link);
        }

        private Link CreateLink(NodeView beginNodeView, NodeView endNodeView) {
            return new Link(ref beginNodeView, ref endNodeView);
        }

        public void Clear() {
            foreach (var link in AddedLinks) {
                link.Dispose();
            }
            AddedLinks.Clear();

            foreach (var nodeView in AddedNodeViews) {
                nodeView.Dispose();
            }
            AddedNodeViews.Clear();

            _selectedNodeView = null;
            Refresh();
        }

        #region IXmlSerializable

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            var clientNodeViewSerializer = new XmlSerializer(typeof(List<ClientNodeView>));
            var networkNodeViewSerializer = new XmlSerializer(typeof(List<NetworkNodeView>));
            var linkSerializer = new XmlSerializer(typeof(List<Link>));

            reader.ReadStartElement(nameof(EditorPanel));
            var clientNodeViews =
                clientNodeViewSerializer.Deserialize(reader) as List<ClientNodeView>;
            if (clientNodeViews != null)
                foreach (var clientNodeView in clientNodeViews) Add(clientNodeView);

            var networkNodeViews =
                networkNodeViewSerializer.Deserialize(reader) as List<NetworkNodeView>;
            if (networkNodeViews != null)
                foreach (var networkNodeView in networkNodeViews) Add(networkNodeView);

            foreach (var link in linkSerializer.Deserialize(reader) as List<Link>) {
                RestoreReferences(link);
                Add(link);
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            var clientNodeViewSerializer = new XmlSerializer(typeof(List<ClientNodeView>));
            var networkNodeViewSerializer = new XmlSerializer(typeof(List<NetworkNodeView>));
            var linkSerializer = new XmlSerializer(typeof(List<Link>));

            clientNodeViewSerializer.Serialize(writer,
                AddedNodeViews.OfType<ClientNodeView>().ToList());
            networkNodeViewSerializer.Serialize(writer,
                AddedNodeViews.OfType<NetworkNodeView>().ToList());
            linkSerializer.Serialize(writer, AddedLinks);
        }

        private void RestoreReferences(Link link) {
            var beginNodeViewId = link.Parameters.BeginNodeViewId;
            var endNodeViewId = link.Parameters.EndNodeViewId;

            var beginNodeView = AddedNodeViews.Find(box => box.Id.Equals(beginNodeViewId));
            var endNodeView = AddedNodeViews.Find(box => box.Id.Equals(endNodeViewId));

            link.SetAttachmentNodeViews(ref beginNodeView, ref endNodeView);
        }

        #endregion
    }
}