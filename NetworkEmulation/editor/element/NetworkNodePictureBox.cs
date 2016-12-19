using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using NetworkEmulation.Properties;
using NetworkUtilities.element;
using XmlSerializer = NetworkUtilities.XmlSerializer;

namespace NetworkEmulation.editor.element {
    public class NetworkNodePictureBox : NodePictureBox {
        public NetworkNodePictureBox() {
            Image = Resources.NetworkNodeNotSelected;
        }

        public NetworkNodeSerializableParameters Parameters { get; set; }

        public override Process Initialize() {
            throw new NotImplementedException();
        }

        public override void MarkAsSelected() {
            Image = Resources.NetworkNodeSelected;
        }

        public override void MarkAsDeselected() {
            Image = Resources.NetworkNodeNotSelected;
        }

        public override void MarkAsOnline() {
            Image = Resources.NetworkNodeOnline;
        }

        public override void MarkAsOffline() {
            Image = Resources.NetworkNodeOffline;
        }

        public override void ReadXml(XmlReader reader) {
            base.ReadXml(reader);
            reader.ReadStartElement(nameof(NetworkNodePictureBox));
            Parameters = XmlSerializer.Deserialize<NetworkNodeSerializableParameters>(reader);
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer) {
            base.WriteXml(writer);
            XmlSerializer.Serialize(writer, Parameters);
        }
    }
}