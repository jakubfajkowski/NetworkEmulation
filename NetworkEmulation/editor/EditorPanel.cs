using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NetworkEmulation.editor.element;

namespace NetworkEmulation.editor {
    public enum Mode {
        AddClientNode,
        AddNetworkNode,
        AddLink,
        AddConnection,
        Move,
        Delete
    }

    [Serializable]
    public partial class EditorPanel : UserControl, IXmlSerializable {
        private List<Connection> _addedConnections = new List<Connection>();
        private List<Link> _addedLinks = new List<Link>();
        private readonly List<NodePictureBox> _addedNodePictureBoxes = new List<NodePictureBox>();
        private List<Link> _currentConnectionLinks;
        private NodePictureBox _currentNodePictureBox;

        private bool _handlingAddingConnection;
        private Mode _mode = Mode.Move;

        public EditorPanel() {
            InitializeComponent();
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

        //private static object NewInstance(object obj) {
        //    return Activator.CreateInstance(obj.GetType());

        //}

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            var clientNodePictureBoxSerializer = new XmlSerializer(typeof(List<ClientNodePictureBox>));
            var networkNodePictureBoxSerializer = new XmlSerializer(typeof(List<NetworkNodePictureBox>));
            var linkSerializer = new XmlSerializer(typeof(List<Link>));
            var connectionSerializer = new XmlSerializer(typeof(List<Connection>));

            reader.ReadStartElement(nameof(EditorPanel));
            var clientNodePictureBoxes = clientNodePictureBoxSerializer.Deserialize(reader) as List<ClientNodePictureBox>;
            if (clientNodePictureBoxes != null) {
                foreach (var clientNodePictureBox in clientNodePictureBoxes) {
                    AddNodePictureBox(clientNodePictureBox);
                }
            }

            var networkNodePictureBoxes = networkNodePictureBoxSerializer.Deserialize(reader) as List<NetworkNodePictureBox>;
            if (networkNodePictureBoxes != null) {
                foreach (var networkNodePictureBox in networkNodePictureBoxes) {
                    AddNodePictureBox(networkNodePictureBox);
                }
            }

            _addedLinks = linkSerializer.Deserialize(reader) as List<Link>;
            _addedConnections = connectionSerializer.Deserialize(reader) as List<Connection>;
            reader.ReadEndElement();

            ShowElements();
        }

        public void WriteXml(XmlWriter writer) {
            var clientNodePictureBoxSerializer = new XmlSerializer(typeof(List<ClientNodePictureBox>));
            var networkNodePictureBoxSerializer = new XmlSerializer(typeof(List<NetworkNodePictureBox>));
            var linkSerializer = new XmlSerializer(typeof(List<Link>));
            var connectionSerializer = new XmlSerializer(typeof(List<Connection>));

            clientNodePictureBoxSerializer.Serialize(writer,
                _addedNodePictureBoxes.OfType<ClientNodePictureBox>().ToList());
            networkNodePictureBoxSerializer.Serialize(writer,
                _addedNodePictureBoxes.OfType<NetworkNodePictureBox>().ToList());
            linkSerializer.Serialize(writer, _addedLinks);
            connectionSerializer.Serialize(writer, _addedConnections);
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
                    var link = CreateLink(CurrentNodePictureBox, sender as NodePictureBox);
                    AddLink(link);
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
            var currentConnection = CreateConnection(currentConnectionLinks);
            AddConnection(currentConnection);
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

        private void AddLink(Link link) {
            Controls.Add(link);
            _addedLinks.Add(link);
        }

        private Link CreateLink(NodePictureBox beginNodePictureBox, NodePictureBox endNodePictureBox) {
            return new Link(ref beginNodePictureBox, ref endNodePictureBox);
        }

        private void AddConnection(Connection connection) {
            _addedConnections.Add(connection);
        }

        private Connection CreateConnection(List<Link> connectionLinks) {
            return new Connection(connectionLinks);
        }

        private void ShowElements() {
            foreach (var link in _addedLinks) {
                RestoreLinkReferences(link);
                AddLink(link);
            }

            foreach (var connection in _addedConnections) {
                RestoreConnectionReferences(connection);
                AddConnection(connection);
            }
        }

        private void RestoreLinkReferences(Link link) {
            var beginNodePictureBoxId = link.Parameters.BeginNodePictureBoxId;
            var endNodePictureBoxId = link.Parameters.EndNodePictureBoxId;

            var beginNodePictureBox = _addedNodePictureBoxes.Find(box => box.Id.Equals(beginNodePictureBoxId));
            var endNodePictureBox = _addedNodePictureBoxes.Find(box => box.Id.Equals(endNodePictureBoxId));

            link.SetAttachmentNodePictureBoxes(ref beginNodePictureBox, ref endNodePictureBox);
        }

        private void RestoreConnectionReferences(Connection connection) {
            var links = _addedLinks.FindAll(link => connection.Parameters.LinksIds.Contains(link.Id));

            connection.Links = links;
        }
    }
}