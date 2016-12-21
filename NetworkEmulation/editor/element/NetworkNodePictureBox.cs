using System;
using System.Diagnostics;
using System.Xml;
using NetworkEmulation.Properties;
using NetworkUtilities;
using NetworkUtilities.element;
using XmlSerializer = NetworkUtilities.XmlSerializer;

namespace NetworkEmulation.editor.element {
    public class NetworkNodePictureBox : NodePictureBox {
        public NetworkNodePictureBox() {
            Image = Resources.NetworkNodeNotSelected;
            Parameters = new NetworkNodeSerializableParameters();
            Parameters.NetworkManagmentSystemDataPort = PortRandomizer.RandomFreePort();
            Parameters.CableCloudDataPort = CableCloudDataPort;
        }

        public NetworkNodeSerializableParameters Parameters { get; set; }

        public override Process Initialize() {
            var process = new Process {
                StartInfo = {
                    #if DEBUG
                    FileName = "..\\..\\..\\NetworkNode\\bin\\Debug\\NetworkNode.exe",
                    #else
                    FileName = "..\\..\\..\\NetworkNode\\bin\\Release\\NetworkNode.exe",
                    #endif
                    Arguments = XmlSerializer.Serialize(Parameters),
                }
            };

            return process;
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