using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    class NetworkCallController : ControlPlaneElement
    {
        Queue<NetworkAddress[]> networkAddresseses = new Queue<NetworkAddress[]>();

        private void DirectoryRequest(string[] clientNames) {
            var directioryRequest = new SignallingMessage(SignallingMessageOperation.DirectoryRequest, clientNames);
            SendMessage(directioryRequest);
        }

        private void DirectoryRequest(NetworkAddress[] clientAddress) {
            var directioryRequest = new SignallingMessage(SignallingMessageOperation.DirectoryRequest, clientAddress);
            SendMessage(directioryRequest);
        }

        private void CallCoordination(NetworkAddress[] clientAddresses) {
            var callCoordination = new SignallingMessage(SignallingMessageOperation.CallCoordination, clientAddresses);
            SendMessage(callCoordination);
        }

        private void ConnectionRequest(NetworkAddress[] clientAddresses) {
            var callCoordination = new SignallingMessage(SignallingMessageOperation.ConnectionRequest, clientAddresses);
            SendMessage(callCoordination);
        }

        private void CallAccept(string[] clientNames) {
            var callAccept = new SignallingMessage(SignallingMessageOperation.CallAccept, clientNames);
            SendMessage(callAccept);
        }

        private void NccCallConfirmation(bool confirmed) {
            var callConfirmation = new SignallingMessage(SignallingMessageOperation.NccCallConfirmation, confirmed);
            SendMessage(callConfirmation);
        }

        public override void RecieveMessage(SignallingMessage message) {

            switch (message.Operation)
            {
                case SignallingMessageOperation.CallRequest:
                    DirectoryRequest((string[])message.Payload);
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
                    if (message.Payload is NetworkAddress[]) {
                        networkAddresseses.Enqueue((NetworkAddress[])message.Payload);
                        CallCoordination((NetworkAddress[])message.Payload);
                    } else if (message.Payload is string[]) {
                        CallAccept((string[])message.Payload);
                    }
                    
                    break;
                case SignallingMessageOperation.CallCoordinationResponse:

                    break;
                case SignallingMessageOperation.ConnectionRequestResponse:

                    break;
                case SignallingMessageOperation.NccCallConfirmation:
                    if ((bool) message.Payload) {
                        ConnectionRequest(networkAddresseses.Dequeue());
                    }
                    break;
                case SignallingMessageOperation.CpccCallConfirmation:
                    if ((bool)message.Payload) {
                        NccCallConfirmation(true);
                    }
                    break;
            }
        }
    }
}
