﻿using System.Diagnostics;
using System.Xml;
using NetworkEmulation.Properties;
using NetworkUtilities.Network.NetworkNode;
using NetworkUtilities.Utilities.Serialization;

namespace NetworkEmulation.Workplace.Element {
    public class NetworkNodeView : NodeView {
        public NetworkNodeView() {
            Image = Resources.NetworkNodeNotSelected;
            Parameters = new NetworkNodeModel();
            Parameters.CableCloudListeningPort = Settings.Default.CableCloudListenerPort;
        }

        public int GetFreePort() {
            return ++Parameters.NumberOfPorts;
        }

        public bool DoubleClickEnabled { get; set; }

        public NetworkNodeModel Parameters { get; set; }

        public override Process Initialize() {
            var process = new Process {
                StartInfo = {
#if DEBUG
                    FileName = "..\\..\\..\\NetworkNode\\bin\\Debug\\NetworkNode.exe",
#else
                    FileName = "..\\..\\..\\NetworkNode\\bin\\Release\\NetworkNode.exe",
                    #endif
                    Arguments = XmlSerializer.Serialize(Parameters),
                    WindowStyle = ProcessWindowStyle.Minimized
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

        #region IXmlSerializable

        public override void ReadXml(XmlReader reader) {
            base.ReadXml(reader);
            reader.ReadStartElement(nameof(NetworkNodeView));
            Parameters = XmlSerializer.Deserialize<NetworkNodeModel>(reader);
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