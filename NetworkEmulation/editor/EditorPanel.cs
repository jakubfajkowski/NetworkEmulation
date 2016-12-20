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
        private Connection _currentConnection;
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
                _currentConnection = null;
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
                if (_handlingAddingConnection) _currentConnection = new Connection();
                else _currentConnection = null;
            }
        }

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
                    Add(clientNodePictureBox);
                }
            }

            var networkNodePictureBoxes = networkNodePictureBoxSerializer.Deserialize(reader) as List<NetworkNodePictureBox>;
            if (networkNodePictureBoxes != null) {
                foreach (var networkNodePictureBox in networkNodePictureBoxes) {
                    Add(networkNodePictureBox);
                }
            }

            foreach (var link in linkSerializer.Deserialize(reader) as List<Link>) {
                Add(link);
                RestoreReferences(link);
            }

            foreach (var connection in connectionSerializer.Deserialize(reader) as List<Connection>) {
                Add(connection);  
                RestoreReferences(connection); 
            }
            reader.ReadEndElement();

            //ShowElements();
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
                    new LinkForm(link).ShowDialog(this);
                    Add(link);
                    link.MarkAsDeselected();
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
                    EndHandlingAddingConnection();
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
            new ConnectionForm(_currentConnection).ShowDialog(this);

            _currentConnection.Add(link);
        }

        private void EndHandlingAddingConnection() {
            var currentConnection = new Connection();
            Add(currentConnection);
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

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            var graphics = e.Graphics;
            foreach (var insertedLink in _addedLinks) insertedLink.DrawLink(graphics);
        }

        private void Add(NodePictureBox nodePictureBox) {
            nodePictureBox.Click += nodePictureBox_Click;
            Controls.Add(nodePictureBox);
            _addedNodePictureBoxes.Add(nodePictureBox);
        }

        private void DeleteNodePictureBox(NodePictureBox nodePictureBox) {
            Controls.Remove(nodePictureBox);
            _addedNodePictureBoxes.Remove(nodePictureBox);
        }

        private void Add(Link link) {
            Controls.Add(link);
            _addedLinks.Add(link);
        }

        private Link CreateLink(NodePictureBox beginNodePictureBox, NodePictureBox endNodePictureBox) {
            return new Link(ref beginNodePictureBox, ref endNodePictureBox);
        }

        private void Add(Connection connection) {
            _addedConnections.Add(connection);
        }

        private void ShowElements() {
            foreach (var link in _addedLinks) {
                RestoreReferences(link);
                Add(link);
            }

            foreach (var connection in _addedConnections) {
                RestoreReferences(connection);
                Add(connection);
            }
        }

        private void RestoreReferences(Link link) {
            var beginNodePictureBoxId = link.Parameters.BeginNodePictureBoxId;
            var endNodePictureBoxId = link.Parameters.EndNodePictureBoxId;

            var beginNodePictureBox = _addedNodePictureBoxes.Find(box => box.Id.Equals(beginNodePictureBoxId));
            var endNodePictureBox = _addedNodePictureBoxes.Find(box => box.Id.Equals(endNodePictureBoxId));

            link.SetAttachmentNodePictureBoxes(ref beginNodePictureBox, ref endNodePictureBox);
        }

        private void RestoreReferences(Connection connection) {
            var links = _addedLinks.FindAll(link => connection.Parameters.LinksIds.Contains(link.Id));

            connection.Links = links;
        }
    }
}