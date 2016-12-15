using System;
using System.Diagnostics;
using NetworkEmulation.Properties;

namespace NetworkEmulation.editor {
    internal class ClientNodePictureBox : NodePictureBox {
        public ClientNodePictureBox() {
            Image = Resources.ClientNodeNotSelected;
        }

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
    }
}