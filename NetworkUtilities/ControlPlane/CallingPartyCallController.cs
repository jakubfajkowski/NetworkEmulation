using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    class CallingPartyCallController : ControlPlaneElement
    {
        private void SendCallRequest(string clientA, string clientZ) {
            string[] clientNames = { clientA, clientZ };
            SignallingMessage callRequest = new SignallingMessage() {
                Operation = SignallingMessageOperation.CallRequest,
                Payload = clientNames
            };
            SendMessage(callRequest);
        }

        private void SendCallTeardown(string clientA, string clientZ) {
            //string[] clientNames = { clientA, clientZ };
            //var callTeardown = activeSession[clientNames];
            //callTeardown.Operation = SignallingMessageOperation.CallTeardown;
            //callTeardown.Payload = clientNames;
            //SendMessage(callTeardown);
        }

        private void SendCallConfirmation(SignallingMessage message) {
            bool confirmed;
            var callConfirmation = message;
            callConfirmation.Operation = SignallingMessageOperation.CallConfirmation;
            //callConfirmation.Payload =
            SendMessage(callConfirmation);
        }

        private void SendCallAcceptResponse(SignallingMessage message) {
            bool confirmed;
            var callAcceptResponse = message;
            callAcceptResponse.Operation = SignallingMessageOperation.CallAcceptResponse;
            //callAcceptResponse.Payload =
            SendMessage(callAcceptResponse);
        }

        private void CallTeardownResponse(SignallingMessage message) {
            bool confirmed;
            var callTeardownResponse = message;
            callTeardownResponse.Operation = SignallingMessageOperation.CallTeardownResponse;
            //callTeardownResponse.Payload =
            SendMessage(callTeardownResponse);
        }

        public override void ReceiveMessage(SignallingMessage message) {

            switch (message.Operation)
            {
                case SignallingMessageOperation.CallAccept:
                    break;
                case SignallingMessageOperation.CallTeardown:
                    break;
                case SignallingMessageOperation.CallRequestResponse:
                    break;
                case SignallingMessageOperation.CallTeardownResponse:
                    break;


            }
        }
    }
}
