using System.Collections.Generic;
using NetworkUtilities.Log;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ControlPlane {
    public abstract class ControlPlaneElement : LogObject {
        public delegate void MessageToSendHandler(object sender, SignallingMessage message);

        protected readonly ControlPlaneElementType ControlPlaneElementType;

        private readonly List<UniqueId> _currentlyHandledSessions = new List<UniqueId>();

        public readonly NetworkAddress LocalAddress;

        protected ControlPlaneElement(NetworkAddress localAddress, ControlPlaneElementType controlPlaneElementType) {
            LocalAddress = localAddress;
            ControlPlaneElementType = controlPlaneElementType;
        }

        public event MessageToSendHandler MessageToSend;

        private void StartSession(UniqueId sessionId) {
            _currentlyHandledSessions.Add(sessionId);
        }

        protected void EndSession(UniqueId sessionId) {
            _currentlyHandledSessions.Remove(sessionId);
        }

        protected void SendMessage(SignallingMessage message) {
            message.SourceAddress = LocalAddress;
            message.SourceControlPlaneElement = ControlPlaneElementType;

            OnUpdateState("[OUT] " + message);
            MessageToSend?.Invoke(this, message);
        }

        public virtual void ReceiveMessage(SignallingMessage message) {
            OnUpdateState("[IN]  " + message);
            var sessionId = message.SessionId;
            if (!IsCurrentlyHandled(sessionId)) StartSession(sessionId);
        }

        protected bool IsCurrentlyHandled(UniqueId sessionId) {
            return _currentlyHandledSessions.Contains(sessionId);
        }

        protected override void OnUpdateState(string state) {
            var controlPlaneElementState = $"[{ControlPlaneElementType}] {state}";
            base.OnUpdateState(controlPlaneElementState);
        }
    }
}