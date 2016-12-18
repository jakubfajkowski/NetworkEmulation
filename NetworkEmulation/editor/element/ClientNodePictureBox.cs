using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using NetworkEmulation.Properties;
using NetworkUtilities.element;

namespace NetworkEmulation.editor.element {
    public class ClientNodePictureBox : NodePictureBox {
        public ClientNodePictureBox() {
            Image = Resources.ClientNodeNotSelected;
        }

        public ClientNodeSerializableParameters Parameters { get; set; }

        public override Process Initialize() {
            throw new NotImplementedException();
        }

        public override void MarkAsSelected() {
            Image = Resources.ClientNodeSelected;
        }

        public override void MarkAsDeselected() {
            Image = Resources.ClientNodeNotSelected;
        }

        public override void MarkAsOnline() {
            Image = Resources.ClientNodeOnline;
        }

        public override void MarkAsOffline() {
            Image = Resources.ClientNodeOffline;
        }

        public override void ReadXml(XmlReader reader) {
            var parametersSerializer = new XmlSerializer(typeof(ClientNodeSerializableParameters));

            reader.ReadStartElement(nameof(ClientNodePictureBox));
            base.ReadXml(reader);
            Parameters = parametersSerializer.Deserialize(reader) as ClientNodeSerializableParameters;
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer) {
            var parametersSerializer = new XmlSerializer(typeof(ClientNodeSerializableParameters));

            base.WriteXml(writer);
            parametersSerializer.Serialize(writer, Parameters);
        }
    }
}