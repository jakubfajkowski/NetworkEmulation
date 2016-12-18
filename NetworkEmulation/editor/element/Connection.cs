using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NetworkEmulation.network.element;

namespace NetworkEmulation.editor.element {
    public class Connection : IMarkable, IInitializable, IXmlSerializable {
        private readonly List<Link> _links;

        public Connection(List<Link> links) {
            _links = links;

            MarkAsSelected();
        }

        public ConnectionSerializableParameters Parameters { get; set; }

        public Process Initialize() {
            throw new NotImplementedException();
        }

        public void MarkAsSelected() {
            _links.ForEach(link => link.MarkAsSelected());
        }

        public void MarkAsDeselected() {
            _links.ForEach(link => link.MarkAsDeselected());
        }

        public void MarkAsOnline() {
            _links.ForEach(link => link.MarkAsOnline());
        }

        public void MarkAsOffline() {
            _links.ForEach(link => link.MarkAsOffline());
        }

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            var parametersSerializer = new XmlSerializer(typeof(ConnectionSerializableParameters));

            reader.ReadStartElement(nameof(Link));
            Parameters = parametersSerializer.Deserialize(reader) as ConnectionSerializableParameters;
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            var parametersSerializer = new XmlSerializer(typeof(ConnectionSerializableParameters));

            parametersSerializer.Serialize(writer, Parameters);
        }
    }
}