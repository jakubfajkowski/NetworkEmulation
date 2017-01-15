using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    class NetworkCallController : ControlPlaneElement
    {
        private readonly Queue<NetworkAddress[]> _networkAddresseses = new Queue<NetworkAddress[]>();

        private void SendDirectoryRequest(SignallingMessage message) {
            var directioryRequest = message;
            directioryRequest.Operation = SignallingMessageOperation.DirectoryRequest;
            directioryRequest.Payload = (string[]) message.Payload;
            SendMessage(directioryRequest);
        }

        private void SendCallCoordination(SignallingMessage message) {
            var callCoordination = message;
            callCoordination.Operation = SignallingMessageOperation.CallCoordination;
            callCoordination.Payload = (NetworkAddress[]) message.Payload;
            SendMessage(callCoordination);
        }

        private void SendConnectionRequest(SignallingMessage message) {
            var connectionRequest = message;
            connectionRequest.Operation = SignallingMessageOperation.ConnectionRequest;
            connectionRequest.Payload = (NetworkAddress[])message.Payload;
            SendMessage(connectionRequest);
        }

        private void SendCallAccept(SignallingMessage message) {
            var callAccept = message;
            callAccept.Operation = SignallingMessageOperation.CallAccept;
            callAccept.Payload = message.Payload;
            SendMessage(callAccept);
        }

        private void SendCallConfirmation(SignallingMessage message) {
            var callConfirmation = message;
            callConfirmation.Operation = SignallingMessageOperation.CallConfirmation;
            callConfirmation.Payload = message.Payload;
            SendMessage(callConfirmation);
        }

        private void SendCallCoordinationResponse(SignallingMessage message) {
            var callCoordinationResponse = message;
            callCoordinationResponse.Operation = SignallingMessageOperation.CallCoordinationResponse;
            callCoordinationResponse.Payload = message.Payload;
            SendMessage(callCoordinationResponse);
        }

        private void SendCallRequestResponse(SignallingMessage message) {
            var callRequestResponse = message;
            callRequestResponse.Operation = SignallingMessageOperation.CallRequestResponse;
            callRequestResponse.Payload = message.Payload;
            SendMessage(callRequestResponse);
        }

        private void SendCallTeardownResponse(SignallingMessage message) {
            var callTeardownResponse = message;
            callTeardownResponse.Operation = SignallingMessageOperation.CallTeardownResponse;
            callTeardownResponse.Payload = message.Payload;
            SendMessage(callTeardownResponse);
        }

        public override void RecieveMessage(SignallingMessage message) {

            switch (message.Operation)
            {
                case SignallingMessageOperation.CallRequest:
                    SendDirectoryRequest(message);
                    break;
                case SignallingMessageOperation.CallTeardown:

                    break;
                case SignallingMessageOperation.CallCoordination:
                    
                    break;
                case SignallingMessageOperation.CallTeardownResponse:

                    break;
                case SignallingMessageOperation.CallAcceptResponse:

                    break;
                case SignallingMessageOperation.DirectoryResponse:
                    //warunek po networkAddress czy wysłać callCoordination czy CallAccept
                    //if (message.Payload is NetworkAddress[]) {
                    //    _networkAddresseses.Enqueue((NetworkAddress[])message.Payload);
                    //    CallCoordination((NetworkAddress[])message.Payload);
                    //} else if (message.Payload is string[]) {
                    //    CallAccept((string[])message.Payload);
                    //}
                    
                    break;
                case SignallingMessageOperation.CallCoordinationResponse:

                    break;
                case SignallingMessageOperation.ConnectionRequestResponse:

                    break;
                case SignallingMessageOperation.CallConfirmation:
                    //if ((bool) message.Payload) {
                    //    ConnectionRequest(_networkAddresseses.Dequeue());
                    //    SendCallConfirmation(true);
                    //}
                    break;
            }
        }
    }
}
