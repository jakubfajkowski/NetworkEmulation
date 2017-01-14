using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    class ControlPlaneElement {
        public delegate void MessageReceivedHandler(object sender, SignallingMessage message);
        public delegate void MessageToSendHandler(object sender, SignallingMessage message);

        public event MessageReceivedHandler OnMessageReceived;
        public event MessageReceivedHandler OnMessageToSend;

        protected void ReceiveMessage(SignallingMessage message) {
            OnMessageReceived?.Invoke(this, message);
        }
        protected void SendMessage(SignallingMessage message) {
            OnMessageToSend?.Invoke(this, message);
        }
    }
}
