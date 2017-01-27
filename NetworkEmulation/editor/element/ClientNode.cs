using System.Diagnostics;
using System.Xml;
using NetworkEmulation.Properties;
using NetworkUtilities;
using NetworkUtilities.Element;
using NetworkUtilities.Serialization;

namespace NetworkEmulation.Editor.Element {
    public class ClientNode : NodePictureBox {
        public ClientNode() {
            Image = Resources.ClientNodeNotSelected;
            Parameters = new ClientNodeModel();
            CableCloudDataPort = PortRandomizer.RandomFreePort();
            Parameters.CableCloudDataPort = CableCloudDataPort;
        }

        public ClientNodeModel Parameters { get; set; }

        public override Process Initialize() {
            var process = new Process {
                StartInfo = {
#if DEBUG
                    FileName = "..\\..\\..\\ClientNode\\bin\\Debug\\ClientNode.exe",
#else
                    FileName = "..\\..\\..\\ClientNode\\bin\\Release\\ClientNode.exe",
                    #endif
                    Arguments = XmlSerializer.Serialize(Parameters),
                    UseShellExecute = false
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
            reader.ReadStartElement(nameof(ClientNode));
            Parameters = XmlSerializer.Deserialize<ClientNodeModel>(reader);
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer) {
            base.WriteXml(writer);
            XmlSerializer.Serialize(writer, Parameters);
        }
    }
}