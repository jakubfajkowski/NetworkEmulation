using System.Diagnostics;
using System.Xml;
using NetworkEmulation.Properties;
using NetworkUtilities.Network.ClientNode;
using NetworkUtilities.Utilities;
using NetworkUtilities.Utilities.Serialization;

namespace NetworkEmulation.Workplace.Element {
    public class ClientNodeView : NodeView {
        public ClientNodeView() {
            Image = Resources.ClientNodeNotSelected;
            Parameters = new ClientNodeModel();
            CableCloudDataPort = PortRandomizer.RandomFreePort();
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

        #region IXmlSerializable

        public override void ReadXml(XmlReader reader) {
            base.ReadXml(reader);
            reader.ReadStartElement(nameof(ClientNodeView));
            Parameters = XmlSerializer.Deserialize<ClientNodeModel>(reader);
            NetworkAddress = Parameters.NetworkAddress;
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer) {
            base.WriteXml(writer);
            XmlSerializer.Serialize(writer, Parameters);
        }

        #endregion
    }
}