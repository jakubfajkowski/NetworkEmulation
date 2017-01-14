using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    class CallingPartyCallController : ControlPlaneElement
    {

        private void CallRequest(string clientA, string clientZ) {
            string[] clientNames = { clientA, clientZ };
            SignallingMessage callRequest = new SignallingMessage(SignallingMessageOperation.CallRequest, clientNames);
            SendMessage(callRequest);
        }

        private void CallTeardown(string clientA, string clientZ) {
            string[] clientNames = { clientA, clientZ };
            SignallingMessage callTeardown = new SignallingMessage(SignallingMessageOperation.CallTeardown, clientNames);
            SendMessage(callTeardown);
        }

        private void CpccCallConfirmation(bool confirmed) {
            var callConfirmation = new SignallingMessage(SignallingMessageOperation.CpccCallConfirmation, confirmed);
            SendMessage(callConfirmation);
        }

        public override void RecieveMessage(SignallingMessage message) {

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
