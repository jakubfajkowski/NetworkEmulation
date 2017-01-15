using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    class CallingPartyCallController : ControlPlaneElement
    {
        private bool callConfirmed { get; set; }

        private void SendCallRequest(string clientA, string clientZ) {
            string[] clientNames = { clientA, clientZ };
            SignallingMessage callRequest = new SignallingMessage() {
                Operation = SignallingMessageOperation.CallRequest,
                Payload = clientNames
            };
            SendMessage(callRequest);
        }

        private void SendCallTeardown(string clientA, string clientZ) {
            string[] clientNames = { clientA, clientZ };
            SignallingMessage callTeardown = new SignallingMessage() {
                Operation = SignallingMessageOperation.CallTeardown,
                Payload = clientNames
            };
            SendMessage(callTeardown);
        }

        private void SendCallConfirmation(SignallingMessage message, bool confirmation) {
            var callConfirmation = message;
            callConfirmation.Operation = SignallingMessageOperation.CallConfirmation;
            callConfirmation.Payload = confirmation;
            SendMessage(callConfirmation);
        }

        private void SendCallAcceptResponse(SignallingMessage message) {
            var callAcceptResponse = message;
            callAcceptResponse.Operation = SignallingMessageOperation.CallAcceptResponse;
            callAcceptResponse.Payload = (bool) true; 
            SendMessage(callAcceptResponse);
        }

        private void SendCallTeardownResponse(SignallingMessage message) {
            var callTeardownResponse = message;
            callTeardownResponse.Operation = SignallingMessageOperation.CallTeardownResponse;
            callTeardownResponse.Payload = (bool) true;
            SendMessage(callTeardownResponse);
        }

        public override void RecieveMessage(SignallingMessage message) {
            base.RecieveMessage(message);

            switch (message.Operation)
            {
                case SignallingMessageOperation.CallAccept:
                    SendCallAcceptResponse(message);
                    SendCallConfirmation(message, callConfirmed);
                    break;
                case SignallingMessageOperation.CallTeardown:
                    SendCallTeardownResponse(message);
                    break;
                case SignallingMessageOperation.CallRequestResponse:

                    break;
                case SignallingMessageOperation.CallTeardownResponse:

                    break;
            }
        }
    }
}
