using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    [Serializable]
    public class SignallingMessage {
        public SignallingMessageOperation Operation { get; set; }
        public SignallingMessageDestinationControlPlaneElement DestinationControlPlaneElement { get; set; }
        public UniqueId SessionId { get; private set; }
        public NetworkAddress DestinationAddress { get; set; }
        public NetworkAddress SourceAddress { get; set; }
        public Object Payload { get; set; }

        //Should be used only in CPCC! In other cases pass received SignallingMessage.
        public SignallingMessage() {
            SessionId = UniqueId.Generate();
        }


    }
}
