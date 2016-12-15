using System;
using System.Diagnostics;
using NetworkEmulation.Properties;

namespace NetworkEmulation.editor {
    internal class NetworkNodePictureBox : NodePictureBox {
        public NetworkNodePictureBox() {
            Image = Resources.NetworkNodeNotSelected;
        }

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
    }
}