using System;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    [Serializable]
    public class SignallingMessage {
        //Should be used only in CPCC! In other cases pass received SignallingMessage.
        public SignallingMessage() {
            SessionId = UniqueId.Generate();
        }

        public SignallingMessageOperation Operation { get; set; }
        public SignallingMessageDestinationControlPlaneElement DestinationControlPlaneElement { get; set; }
        public UniqueId SessionId { get; private set; }
        public int DemandedCapacity { get; set; }
        public NetworkAddress DestinationAddress { get; set; }
        public NetworkAddress SourceAddress { get; set; }
        public object Payload { get; set; }

        public override string ToString() {
            return $"signalling message from: {SourceAddress} to: {DestinationAddress} - {DestinationControlPlaneElement} {Operation}";
        }
    }
}