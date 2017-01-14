using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    abstract class ControlPlaneElement {
        public delegate void MessageToSendHandler(object sender, SignallingMessage message);

        public event MessageToSendHandler OnMessageToSend;

        protected void SendMessage(SignallingMessage message) {
            OnMessageToSend?.Invoke(this, message);
        }

        public abstract void RecieveMessage(SignallingMessage message);
    }
}
