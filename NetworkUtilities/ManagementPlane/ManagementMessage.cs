using System;

namespace NetworkUtilities.ManagementPlane {
    [Serializable]
    class ManagementMessage {
        public ManagementMessageType Type { get; }
        public object Payload { get; }

        public ManagementMessage(ManagementMessageType type, object payload) {
            Type = type;
            Payload = payload;
        }
    }
}
