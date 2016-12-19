using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using NetworkEmulation.Properties;
using NetworkUtilities.element;
using XmlSerializer = NetworkUtilities.XmlSerializer;

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
            base.ReadXml(reader);
            reader.ReadStartElement(nameof(ClientNodePictureBox));
            XmlSerializer.Deserialize<ClientNodeSerializableParameters>(reader);
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer) {
            base.WriteXml(writer);
            XmlSerializer.Serialize(writer, Parameters);
        }
    }
}