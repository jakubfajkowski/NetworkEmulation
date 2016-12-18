using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using NetworkEmulation.Properties;
using NetworkUtilities.element;

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
            var parametersSerializer = new XmlSerializer(typeof(NetworkNodeSerializableParameters));

            reader.ReadStartElement(nameof(NetworkNodePictureBox));
            base.ReadXml(reader);
            Parameters = parametersSerializer.Deserialize(reader) as NetworkNodeSerializableParameters;
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer) {
            var parametersSerializer = new XmlSerializer(typeof(NetworkNodeSerializableParameters));

            base.WriteXml(writer);
            parametersSerializer.Serialize(writer, Parameters);
        }
    }
}