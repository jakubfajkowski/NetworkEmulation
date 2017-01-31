using System;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    [Serializable]
    public class SignallingMessage {
        //Should be used only in CPCC! In other cases pass received SignallingMessage.
        public SignallingMessage() {
            SessionId = UniqueId.Generate();
        }

        public UniqueId SessionId { get; private set; }

        public NetworkAddress DestinationAddress { get; set; }
        public ControlPlaneElementType DestinationControlPlaneElement { get; set; }
        public NetworkAddress SourceAddress { get; set; }
        public ControlPlaneElementType SourceControlPlaneElement { get; set; }
        public OperationType Operation { get; set; }
        public int DemandedCapacity { get; set; }

        public object Payload { get; set; }

        public override string ToString() {
            return
                $"{SourceAddress}.{SourceControlPlaneElement}->{DestinationAddress}.{DestinationControlPlaneElement} {Operation}";
        }
    }
}