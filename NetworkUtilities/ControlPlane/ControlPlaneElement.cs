using System.Collections.Generic;

namespace NetworkUtilities.ControlPlane {
    public abstract class ControlPlaneElement {
        public NetworkAddress SourceAddress { get; private set; }

        public delegate void MessageToSendHandler(object sender, SignallingMessage message);

        private readonly List<UniqueId> _currentlyHandledSessions = new List<UniqueId>();

        public event MessageToSendHandler OnMessageToSend;

        protected ControlPlaneElement(NetworkAddress networkAddress) {
            SourceAddress = networkAddress;
        }

        protected void SendMessage(SignallingMessage message) {
            message.SourceAddress = SourceAddress;
            OnMessageToSend?.Invoke(this, message);
        }

        protected bool IsCurrentlyHandled(UniqueId sessionId) {
            return _currentlyHandledSessions.Contains(sessionId);
        }

        private void StartSession(UniqueId sessionId) {
            _currentlyHandledSessions.Add(sessionId);
        }

        protected void EndSession(UniqueId sessionId) {
            _currentlyHandledSessions.Remove(sessionId);
        }

        public virtual void ReceiveMessage(SignallingMessage message) {
            var sessionId = message.SessionId;

            if (!IsCurrentlyHandled(sessionId)) {
                StartSession(sessionId);
            }
        }
    }
}
