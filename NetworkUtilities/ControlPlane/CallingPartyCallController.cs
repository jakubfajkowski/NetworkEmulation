using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    class CallingPartyCallController : ControlPlaneElement
    {
        public bool callConfirmed { get; set; }
        public NetworkAddress nccAddress { get; set; }

        private void SendCallRequest(string clientA, string clientZ) {
            string[] clientNames = { clientA, clientZ };
            SignallingMessage callRequest = new SignallingMessage() {
                Operation = SignallingMessageOperation.CallRequest,
                Payload = clientNames,
                DestinationAddress = nccAddress
            };
            SendMessage(callRequest);
        }

        private void SendCallTeardown(string clientA, string clientZ) {
            string[] clientNames = { clientA, clientZ };
            SignallingMessage callTeardown = new SignallingMessage() {
                Operation = SignallingMessageOperation.CallTeardown,
                Payload = clientNames,
                DestinationAddress = nccAddress
            };
            SendMessage(callTeardown);
        }

        private void SendCallConfirmation(SignallingMessage message, bool confirmation) {
            var callConfirmation = message;
            callConfirmation.Operation = SignallingMessageOperation.CallConfirmation;
            callConfirmation.Payload = confirmation;
            callConfirmation.DestinationAddress = nccAddress;
            SendMessage(callConfirmation);
        }

        private void SendCallAcceptResponse(SignallingMessage message) {
            var callAcceptResponse = message;
            callAcceptResponse.Operation = SignallingMessageOperation.CallAcceptResponse;
            callAcceptResponse.Payload = (bool) true;
            callAcceptResponse.DestinationAddress = nccAddress;
            SendMessage(callAcceptResponse);
        }

        private void SendCallTeardownResponse(SignallingMessage message) {
            var callTeardownResponse = message;
            callTeardownResponse.Operation = SignallingMessageOperation.CallTeardownResponse;
            callTeardownResponse.Payload = (bool) true;
            callTeardownResponse.DestinationAddress = nccAddress;
            SendMessage(callTeardownResponse);
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

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
