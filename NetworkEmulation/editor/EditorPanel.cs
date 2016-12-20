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
        public List<Connection> _addedConnections { get; private set; } = new List<Connection>();
        public List<Link> _addedLinks { get; private set; }  = new List<Link>();
        public List<NodePictureBox> _addedNodePictureBoxes { get; private set; }  = new List<NodePictureBox>();

        private Connection _currentConnection;
        private NodePictureBox _currentNodePictureBox;
        private NodePictureBox CurrentNodePictureBox {
            get { return _currentNodePictureBox; }
            set {
                Deselect(_currentNodePictureBox);

                _currentNodePictureBox = value;

                Select(_currentNodePictureBox);
            }
        }

        private bool _handlingAddingConnection;
        private Mode _mode = Mode.Move;
        private bool HandlingAddingConnection {
            get { return _handlingAddingConnection; }
            set {
                _handlingAddingConnection = value;
                _currentConnection = _handlingAddingConnection ? new Connection() : null;
            }
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

        public EditorPanel() {
            InitializeComponent();
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

                case Mode.AddConnection:
                    if (!HandlingAddingConnection) {
                        BeginHandlingAddingConnection(sender as NodePictureBox);
                    }
                    else {
                        var previousNodePictureBox = CurrentNodePictureBox;
                        CurrentNodePictureBox = sender as NodePictureBox;

                        var networkNodePictureBox = CurrentNodePictureBox as NetworkNodePictureBox;
                        if (networkNodePictureBox != null)
                            new ConnectionForm(_currentConnection, networkNodePictureBox.Parameters.NetworkManagmentSystemDataPort).ShowDialog(this);

                        HandleAddingConnection(previousNodePictureBox, CurrentNodePictureBox);
                    }
                    break;

                case Mode.Delete:
                    MessageBox.Show("Not implemented.");
                    break;

                default:
                    return;
            }
        }

        private void BeginHandlingAddingConnection(NodePictureBox beginNodePictureBox) {
            if (beginNodePictureBox is NetworkNodePictureBox)
                FailHandlingAddingConnection("Connection should begin with client node.");
            if (beginNodePictureBox is ClientNodePictureBox) {
                HandlingAddingConnection = true;
                _currentConnection.BeginClientNodePictureBox = beginNodePictureBox as ClientNodePictureBox;
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
            _currentConnection.Add(link);
        }

        private void EndHandlingAddingConnection() {
            Add(_currentConnection);
            _currentConnection.EndClientNodePictureBox= CurrentNodePictureBox as ClientNodePictureBox;
            _currentConnection.FillClientTable();
            _handlingAddingConnection = false;
        }

        private void FailHandlingAddingConnection(string message) {
            MessageBox.Show(message, "Add Connection Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _handlingAddingConnection = false;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            var graphics = e.Graphics;
            foreach (var insertedLink in _addedLinks) insertedLink.DrawLink(graphics);
        }

        private void Add(NodePictureBox nodePictureBox) {
            nodePictureBox.Click += nodePictureBox_OnClick;
            Controls.Add(nodePictureBox);
            _addedNodePictureBoxes.Add(nodePictureBox);

            Deselect(nodePictureBox);
        }

        private void DeleteNodePictureBox(NodePictureBox nodePictureBox) {
            Controls.Remove(nodePictureBox);
            _addedNodePictureBoxes.Remove(nodePictureBox);
        }

        private void Add(Link link) {
            Controls.Add(link);
            _addedLinks.Add(link);

            Deselect(link);
        }

        private Link CreateLink(NodePictureBox beginNodePictureBox, NodePictureBox endNodePictureBox) {
            return new Link(ref beginNodePictureBox, ref endNodePictureBox);
        }

        private void Add(Connection connection) {
            _addedConnections.Add(connection);

            Select(connection);
        }

        #region IXmlSerializable

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            var clientNodePictureBoxSerializer = new XmlSerializer(typeof(List<ClientNodePictureBox>));
            var networkNodePictureBoxSerializer = new XmlSerializer(typeof(List<NetworkNodePictureBox>));
            var linkSerializer = new XmlSerializer(typeof(List<Link>));
            var connectionSerializer = new XmlSerializer(typeof(List<Connection>));

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

            foreach (var connection in connectionSerializer.Deserialize(reader) as List<Connection>) {
                RestoreReferences(connection);
                Add(connection);
            }
            reader.ReadEndElement();
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

            var beginNodePictureBoxId = connection.Parameters.BeginClientNodePictureBoxId;
            var endNodePictureBoxId = connection.Parameters.EndClientNodePictureBoxId;
            
            var beginNodePictureBox = _addedNodePictureBoxes.Find(box => box.Id.Equals(beginNodePictureBoxId));
            var endNodePictureBox = _addedNodePictureBoxes.Find(box => box.Id.Equals(endNodePictureBoxId));
            connection.BeginClientNodePictureBox = beginNodePictureBox as ClientNodePictureBox;
            connection.EndClientNodePictureBox = endNodePictureBox as ClientNodePictureBox;
           }

        #endregion
    }
}