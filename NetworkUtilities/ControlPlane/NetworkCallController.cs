using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    class NetworkCallController : ControlPlaneElement
    {
        private bool callConfirmed { get; set; }
        //private readonly Queue<NetworkAddress[]> _networkAddresseses = new Queue<NetworkAddress[]>();
        private readonly Dictionary<UniqueId, NetworkAddress[]> _networkAddressDictionary = new Dictionary<UniqueId, NetworkAddress[]>();


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
            callCoordination.DestinationAddress = _networkAddressDictionary[message.SessionId][1].GetParentsAddress();
            SendMessage(callCoordination);
        }

        private void SendConnectionRequest(SignallingMessage message) {
            var connectionRequest = message;
            connectionRequest.Operation = SignallingMessageOperation.ConnectionRequest;
            connectionRequest.Payload = (NetworkAddress[])message.Payload;
            connectionRequest.DestinationAddress = _networkAddressDictionary[message.SessionId][1].GetRootFromBeginning(2);
            SendMessage(connectionRequest);
        }

        private void SendCallAccept(SignallingMessage message) {
            var callAccept = message;
            callAccept.Operation = SignallingMessageOperation.CallAccept;
            callAccept.Payload = message.Payload;
            callAccept.DestinationAddress = _networkAddressDictionary[message.SessionId][1];
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
            callCoordinationResponse.Payload = (bool)true;
            callCoordinationResponse.DestinationAddress =_networkAddressDictionary[message.SessionId][0].GetParentsAddress();
            SendMessage(callCoordinationResponse);
        }

        private void SendCallRequestResponse(SignallingMessage message) {
            var callRequestResponse = message;
            callRequestResponse.Operation = SignallingMessageOperation.CallRequestResponse;
            callRequestResponse.Payload = (bool) true;
            callRequestResponse.DestinationAddress = _networkAddressDictionary[message.SessionId][0];
            SendMessage(callRequestResponse);
        }

        private void SendCallTeardownResponse(SignallingMessage message) {
            var callTeardownResponse = message;
            callTeardownResponse.Operation = SignallingMessageOperation.CallTeardownResponse;
            callTeardownResponse.Payload = (bool)true;
            SendMessage(callTeardownResponse);
        }

        public override void ReceiveMessage(SignallingMessage message) {
            base.ReceiveMessage(message);
            switch (message.Operation)
            {
                case SignallingMessageOperation.CallRequest:
                    SendCallRequestResponse(message);
                    SendDirectoryRequest(message);
                    break;
                case SignallingMessageOperation.CallTeardown:
                    SendCallTeardownResponse(message);
                    break;
                case SignallingMessageOperation.CallCoordination:
                    SendCallCoordinationResponse(message);
                    SendCallAccept(message);
                    break;
                case SignallingMessageOperation.CallTeardownResponse:

                    break;
                case SignallingMessageOperation.CallAcceptResponse:

                    break;
                case SignallingMessageOperation.DirectoryResponseAddress:
                    var networkAdress = (NetworkAddress[]) message.Payload;
                    _networkAddressDictionary.Add(message.SessionId, networkAdress);

                    if (networkAdress[0].GetId(0) == networkAdress[1].GetId(0)) {
                        SendCallAccept(message);
                    }
                    else {
                        SendCallCoordination(message);
                    }

                    break;
                case SignallingMessageOperation.DirectoryResponseName:
                    var clientName = (string[])message.Payload;

                    break;
                case SignallingMessageOperation.CallCoordinationResponse:

                    break;
                case SignallingMessageOperation.ConnectionRequestResponse:

                    break;
                case SignallingMessageOperation.CallConfirmation:
                    if ((bool) message.Payload) {
                        SendConnectionRequest(message);
                    }
                    
                    break;
            }
        }
    }
}
