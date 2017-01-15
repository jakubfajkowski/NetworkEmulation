using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    [Serializable]
    public class SignallingMessage {
        public SignallingMessageOperation Operation { get; set; }
        public UniqueId SessionId { get; private set; }
        public Object Payload { get; set; }

        public SignallingMessage() {
            SessionId = UniqueId.Generate();
        }
    }
}
