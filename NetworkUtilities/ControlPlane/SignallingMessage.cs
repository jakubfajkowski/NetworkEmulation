using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    public class SignallingMessage {
        public SignallingMessageOperation Operation { get; private set; }
        public Object Payload { get; private set; }

        public SignallingMessage(SignallingMessageOperation operation, Object payload) {
            this.Operation = operation;
            this.Payload = payload;
        }
    }
}
