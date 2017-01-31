using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NetworkEmulation.Properties;
using NetworkEmulation.Workplace.Element;

namespace NetworkEmulation.Workplace {
    public enum Mode {
        AddClientNode,
        AddNetworkNode,
        AddLink,
        Move,
        Delete
    }

    [Serializable]
    public partial class EditorPanel : UserControl, IXmlSerializable {
        private Mode _mode = Mode.Move;
        private NodeView _selectedNodeView;

        public EditorPanel() {
            InitializeComponent();
        }

        public List<LinkView> AddedLinks { get; } = new List<LinkView>();
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

                    var parameters = networkNodeView.Parameters;
                    parameters.IpAddress = Settings.Default.IpAddress;
                    parameters.CableCloudListeningPort = Settings.Default.CableCloudListenerPort;
                    parameters.NetworkManagmentSystemListeningPort = Settings.Default.NetworkManagmentSystemListeningPort;
                    parameters.SignallingCloudListeningPort = Settings.Default.SignallingCloudListeningPort;
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
                        var secondNodeView = sender as NodeView;
                        if (SelectedNodeView.Equals(secondNodeView)) {
                            MessageBox.Show("Selected nodes are the same.");
                        }
                        else {
                            var link = CreateLink(SelectedNodeView, secondNodeView);
                            new LinkForm(link).ShowDialog(this);
                            Add(link);
                            link.MarkAsDeselected();
                            SelectedNodeView = null;
                        }
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
                var centerPoint = addedNodeView.CenterPoint();
                DrawTextLine(graphics, new Point(centerPoint.X, centerPoint.Y + addedNodeView.Image.Height / 2), 
                    addedNodeView.NetworkAddress.ToString());

                var clientNodeView = addedNodeView as ClientNodeView;

                if (clientNodeView == null) continue;
                DrawTextLine(graphics, new Point(centerPoint.X, centerPoint.Y + clientNodeView.Image.Height/2 + 10),
                    clientNodeView.Parameters.ClientName);
            }
        }

        private void DrawTextLine(Graphics g, Point textPoint, string text) {
            using (var myFont = new Font("Arial", 8)) {
                var namePoint = new Point(textPoint.X - 3 * text.Length, textPoint.Y);
                g.DrawString(text, myFont, Brushes.Black, namePoint);
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

        private void Add(LinkView link) {
            Controls.Add(link);
            AddedLinks.Add(link);

            Deselect(link);
        }

        private LinkView CreateLink(NodeView beginNodeView, NodeView endNodeView) {
            return new LinkView(ref beginNodeView, ref endNodeView);
        }

        public void Clear() {
            foreach (var link in AddedLinks) link.Dispose();
            AddedLinks.Clear();

            foreach (var nodeView in AddedNodeViews) nodeView.Dispose();
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
            var linkSerializer = new XmlSerializer(typeof(List<LinkView>));

            reader.ReadStartElement(nameof(EditorPanel));
            var clientNodeViews =
                clientNodeViewSerializer.Deserialize(reader) as List<ClientNodeView>;
            if (clientNodeViews != null)
                foreach (var clientNodeView in clientNodeViews) Add(clientNodeView);

            var networkNodeViews =
                networkNodeViewSerializer.Deserialize(reader) as List<NetworkNodeView>;
            if (networkNodeViews != null)
                foreach (var networkNodeView in networkNodeViews) Add(networkNodeView);

            foreach (var link in linkSerializer.Deserialize(reader) as List<LinkView>) {
                RestoreReferences(link);
                Add(link);
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            var clientNodeViewSerializer = new XmlSerializer(typeof(List<ClientNodeView>));
            var networkNodeViewSerializer = new XmlSerializer(typeof(List<NetworkNodeView>));
            var linkSerializer = new XmlSerializer(typeof(List<LinkView>));

            clientNodeViewSerializer.Serialize(writer,
                AddedNodeViews.OfType<ClientNodeView>().ToList());
            networkNodeViewSerializer.Serialize(writer,
                AddedNodeViews.OfType<NetworkNodeView>().ToList());
            linkSerializer.Serialize(writer, AddedLinks);
        }

        private void RestoreReferences(LinkView link) {
            var beginNodeViewId = link.Parameters.BeginNodeViewId;
            var endNodeViewId = link.Parameters.EndNodeViewId;

            var beginNodeView = AddedNodeViews.Find(box => box.Id.Equals(beginNodeViewId));
            var endNodeView = AddedNodeViews.Find(box => box.Id.Equals(endNodeViewId));

            link.SetAttachmentNodeViews(ref beginNodeView, ref endNodeView);
        }

        #endregion
    }
}