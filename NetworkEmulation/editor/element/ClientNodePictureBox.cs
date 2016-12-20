using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using NetworkEmulation.Properties;
using NetworkUtilities;
using NetworkUtilities.element;
using XmlSerializer = NetworkUtilities.XmlSerializer;

namespace NetworkEmulation.editor.element {
    public class ClientNodePictureBox : NodePictureBox {
        public ClientNodePictureBox() {
            Image = Resources.ClientNodeNotSelected;
            Parameters = new ClientNodeSerializableParameters();
            CableCloudDataPort = PortRandomizer.RandomFreePort();
            Parameters.CableCloudDataPort = CableCloudDataPort;
        }

        public ClientNodeSerializableParameters Parameters { get; set; }

        public override Process Initialize() {
            var process = new Process {
                StartInfo = {
                    FileName = "..\\..\\..\\ClientNode\\bin\\Debug\\ClientNode.exe",
                    Arguments = XmlSerializer.Serialize(Parameters)
                }
            };
            return process;
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