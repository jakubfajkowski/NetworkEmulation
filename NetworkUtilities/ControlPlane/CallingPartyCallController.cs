using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    public class CallingPartyCallController : ControlPlaneElement
    {
        public CallingPartyCallController(NetworkAddress networkAddress) : base(networkAddress) {
        }

        public bool callConfirmed { get; set; }
        public NetworkAddress NccAddress { get; private set; }

        public void SendCallRequest(string clientA, string clientZ, NetworkAddress nccAddress, int capacity) {
            NccAddress = nccAddress;
            string[] clientNames = {clientA, clientZ};
            object[] callRequestMessage = { clientNames, capacity };
            SignallingMessage callRequest = new SignallingMessage() {
                Operation = SignallingMessageOperation.CallRequest,
                Payload = callRequestMessage,
                DestinationAddress = nccAddress
            };
            SendMessage(callRequest);
        }

        public void SendCallTeardown(string clientA, string clientZ) {
            string[] clientNames = { clientA, clientZ };
            SignallingMessage callTeardown = new SignallingMessage() {
                Operation = SignallingMessageOperation.CallTeardown,
                Payload = clientNames,
                DestinationAddress = NccAddress
            };
            SendMessage(callTeardown);
        }

        private void SendCallConfirmation(SignallingMessage message, bool confirmation) {
            var callConfirmation = message;
            callConfirmation.Operation = SignallingMessageOperation.CallConfirmation;
            callConfirmation.Payload = confirmation;
            callConfirmation.DestinationAddress = message.SourceAddress;
            SendMessage(callConfirmation);
        }

        private void SendCallAcceptResponse(SignallingMessage message) {
            var callAcceptResponse = message;
            callAcceptResponse.Operation = SignallingMessageOperation.CallAcceptResponse;
            callAcceptResponse.Payload = (bool) true;
            callAcceptResponse.DestinationAddress = message.SourceAddress;
            SendMessage(callAcceptResponse);
        }

        private void SendCallTeardownResponse(SignallingMessage message) {
            var callTeardownResponse = message;
            callTeardownResponse.Operation = SignallingMessageOperation.CallTeardownResponse;
            callTeardownResponse.Payload = (bool) true;
            callTeardownResponse.DestinationAddress = NccAddress;
            SendMessage(callTeardownResponse);
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);

            switch (message.Operation)
            {
                case SignallingMessageOperation.CallAccept:
                    //SendCallAcceptResponse(message);
                    callConfirmed = false;
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
