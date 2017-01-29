using System.Collections.Generic;
using NetworkUtilities.Log;

namespace NetworkUtilities.ControlPlane {
    public abstract class ControlPlaneElement : LogObject {
        public delegate void MessageToSendHandler(object sender, SignallingMessage message);

        private readonly List<UniqueId> _currentlyHandledSessions = new List<UniqueId>();

        protected ControlPlaneElement(NetworkAddress networkAddress) {
            Address = networkAddress;
        }

        public NetworkAddress Address { get; }

        public event MessageToSendHandler MessageToSend;

        protected void SendMessage(SignallingMessage message) {
            message.SourceAddress = Address;
            OnUpdateState(message.ToString());
            MessageToSend?.Invoke(this, message);
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

            if (!IsCurrentlyHandled(sessionId)) StartSession(sessionId);
        }
    }
}