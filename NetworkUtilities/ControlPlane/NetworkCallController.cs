using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    class NetworkCallController : ControlPlaneElement
    {
        private readonly Dictionary<UniqueId, NetworkAddress[]> _networkAddressDictionary = new Dictionary<UniqueId, NetworkAddress[]>();
        private readonly Dictionary<UniqueId, string[]> _nameDictionary = new Dictionary<UniqueId, string[]>();
        private readonly Dictionary<UniqueId, NetworkAddress[]> _snppDictionary = new Dictionary<UniqueId, NetworkAddress[]>();
        private readonly Dictionary<UniqueId, bool> _waitingForConfirmation = new Dictionary<UniqueId, bool>();

        private void SendDirectoryAddressRequest(SignallingMessage message) {
            var directioryRequest = message;
            directioryRequest.Operation = SignallingMessageOperation.DirectoryAddressRequest;
            directioryRequest.Payload = (string[]) message.Payload;
            //callCoordination.DestinationAddress = Directory address??
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
            var snpp = _snppDictionary[message.SessionId];
            connectionRequest.Operation = SignallingMessageOperation.ConnectionRequest;
            connectionRequest.Payload = snpp;
            connectionRequest.DestinationAddress = _networkAddressDictionary[message.SessionId][1].GetRootFromBeginning(1);
            SendMessage(connectionRequest);
        }

        private void SendCallAccept(SignallingMessage message) {
            var callAccept = message;
            var clientNames = _nameDictionary[message.SessionId];
            callAccept.Operation = SignallingMessageOperation.CallAccept;
            callAccept.Payload = clientNames;
            callAccept.DestinationAddress = _networkAddressDictionary[message.SessionId][1];
            SendMessage(callAccept);
        }

        private void SendCallConfirmationToNCC(SignallingMessage message) {
            var callConfirmation = message;
            var clientAddresses = _networkAddressDictionary[message.SessionId];
            callConfirmation.Operation = SignallingMessageOperation.CallConfirmation;
            callConfirmation.Payload = clientAddresses;
            callConfirmation.DestinationAddress = _networkAddressDictionary[message.SessionId][0].GetRootFromBeginning(0);
            SendMessage(callConfirmation);
        }

        private void SendCallConfirmationToCPCC(SignallingMessage message) {
            var callConfirmation = message;
            var clientNames = _nameDictionary[message.SessionId];
            callConfirmation.Operation = SignallingMessageOperation.CallConfirmation;
            callConfirmation.Payload = clientNames;
            callConfirmation.DestinationAddress = _networkAddressDictionary[message.SessionId][0];
            SendMessage(callConfirmation);
        }

        private void SendCallCoordinationResponse(SignallingMessage message) {
            var callCoordinationResponse = message;
            callCoordinationResponse.Operation = SignallingMessageOperation.CallCoordinationResponse;
            callCoordinationResponse.Payload = (bool)true;
            callCoordinationResponse.DestinationAddress =_networkAddressDictionary[message.SessionId][0].GetRootFromBeginning(0);
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
                    var clientNames = (string[]) message.Payload;
                    _nameDictionary.Add(message.SessionId, clientNames);
                    _waitingForConfirmation[message.SessionId] = false;

                    SendCallRequestResponse(message);
                    SendDirectoryAddressRequest(message);
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
                case SignallingMessageOperation.DirectorySnppResponse:
                    var snpp = (NetworkAddress[]) message.Payload;
                    _snppDictionary.Add(message.SessionId, snpp);
                    break;
                case SignallingMessageOperation.DirectoryAddressResponse:
                    var networkAdress = (NetworkAddress[]) message.Payload;
                    _networkAddressDictionary.Add(message.SessionId, networkAdress);

                    if (networkAdress[0].GetId(0) == networkAdress[1].GetId(0)) {
                        SendCallAccept(message);
                    }
                    else {
                        SendCallCoordination(message);
                    }
                    break;
                case SignallingMessageOperation.DirectoryNameResponse:
                    var clientName = (string[])message.Payload;
                    _nameDictionary.Add(message.SessionId, clientName);
                    break;
                case SignallingMessageOperation.CallCoordinationResponse:
                    
                    break;
                case SignallingMessageOperation.CallAcceptResponse:

                    break;
                case SignallingMessageOperation.ConnectionRequestResponse:

                    break;
                case SignallingMessageOperation.CallConfirmation:
                    if ((bool) message.Payload) {
                        if (!_waitingForConfirmation[message.SessionId]) {
                            var address = _networkAddressDictionary[message.SessionId];
                            if (address[0].GetId(0) == address[1].GetId(0)) {
                                SendConnectionRequest(message);
                                _waitingForConfirmation[message.SessionId] = true;
                            }
                            else {
                                SendCallConfirmationToNCC(message);
                            }
                        }
                        else {
                            SendCallConfirmationToCPCC(message);
                        }
                    }
                    break;
            }
        }
    }
}
