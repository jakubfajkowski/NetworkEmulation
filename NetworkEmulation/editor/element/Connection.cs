using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using NetworkEmulation.Network.Element;
using NetworkUtilities;
using NetworkUtilities.Serialization;
using UniqueId = NetworkUtilities.UniqueId;

namespace NetworkEmulation.Editor.Element {
    public class Connection : IMarkable, ISerializable {
        private ClientNodePictureBox _beginClientNodePictureBox;
        private ClientNodePictureBox _endClientNodePictureBox;

        public Connection() {
            Id = UniqueId.Generate();
            Links = new List<Link>();
            Parameters = new ConnectionModel();
        }

        public List<Link> Links { private get; set; }

        public ClientNodePictureBox BeginClientNodePictureBox {
            private get { return _beginClientNodePictureBox; }
            set {
                _beginClientNodePictureBox = value;
                Parameters.BeginClientNodePictureBoxId = value.Id;
            }
        }

        public ClientNodePictureBox EndClientNodePictureBox {
            private get { return _endClientNodePictureBox; }
            set {
                _endClientNodePictureBox = value;
                Parameters.EndClientNodePictureBoxId = value.Id;
            }
        }

        public ConnectionModel Parameters { get; set; }

        public void MarkAsSelected() {
            Links.ForEach(link => link.MarkAsSelected());
        }

        public void MarkAsDeselected() {
            Links.ForEach(link => link.MarkAsDeselected());
        }

        public void MarkAsOnline() {
            Links.FindAll(
                link =>
                    !(link.BeginNodePictureBox is ClientNodePictureBox) &&
                    !(link.EndNodePictureBox is ClientNodePictureBox)).ForEach(
                link => link.MarkAsOnline());
        }

        public void MarkAsOffline() {
            Links.FindAll(
                link =>
                    !(link.BeginNodePictureBox is ClientNodePictureBox) &&
                    !(link.EndNodePictureBox is ClientNodePictureBox)).ForEach(
                link => link.MarkAsOffline());
        }

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            reader.MoveToContent();
            Id = new UniqueId(reader.GetAttribute("Id"));
            reader.ReadStartElement(nameof(Connection));
            Parameters = XmlSerializer.Deserialize<ConnectionModel>(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteAttributeString("Id", Id.ToString());
            XmlSerializer.Serialize(writer, Parameters);
        }

        public UniqueId Id { get; private set; }

        public void Add(Link link) {
            Links.Add(link);
            Parameters.LinksIds.Add(link.Id);

            link.MarkAsSelected();
        }

        public void FillClientTable() {
            var clientName = EndClientNodePictureBox.Parameters.ClientName;
            var portNumber = Parameters.NodeConnectionInformations[0].InPortNumber;
            var vpi = Parameters.NodeConnectionInformations[0].InVpi;
            var vci = Parameters.NodeConnectionInformations[0].InVci;

            BeginClientNodePictureBox.Parameters.ClientTable.Add(new ClientTableRow(clientName, portNumber, vpi, vci));
        }
    }
}