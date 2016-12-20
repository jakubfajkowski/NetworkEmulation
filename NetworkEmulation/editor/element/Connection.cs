using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using NetworkEmulation.network.element;
using NetworkUtilities;
using UniqueId = NetworkUtilities.UniqueId;
using XmlSerializer = NetworkUtilities.XmlSerializer;

namespace NetworkEmulation.editor.element {
    public class Connection : IMarkable, IInitializable, ISerializable {
        public List<Link> Links { private get; set; }

        public Connection() {
            Id = UniqueId.Generate();
            Links = new List<Link>();
            Parameters = new ConnectionSerializableParameters();
        }

        public void Add(Link link) {
            Links.Add(link);
            link.MarkAsSelected();
        }

        public ConnectionSerializableParameters Parameters { get; set; }

        public Process Initialize() {
            throw new NotImplementedException();
        }

        public void MarkAsSelected() {
            Links.ForEach(link => link.MarkAsSelected());
        }

        public void MarkAsDeselected() {
            Links.ForEach(link => link.MarkAsDeselected());
        }

        public void MarkAsOnline() {
            Links.ForEach(link => link.MarkAsOnline());
        }

        public void MarkAsOffline() {
            Links.ForEach(link => link.MarkAsOffline());
        }

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            reader.MoveToContent();
            Id = new UniqueId(reader.GetAttribute("Id"));
            reader.ReadStartElement(nameof(Connection));
            Parameters = XmlSerializer.Deserialize<ConnectionSerializableParameters>(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteAttributeString("Id", Id.ToString());
            XmlSerializer.Serialize(writer, Parameters);
        }

        public UniqueId Id { get; private set; }
    }
}