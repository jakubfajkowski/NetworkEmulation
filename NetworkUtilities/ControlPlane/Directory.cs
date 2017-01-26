using System.Collections.Generic;
using System.Linq;

namespace NetworkUtilities.ControlPlane {
    internal class Directory : ControlPlaneElement {
        private readonly Dictionary<string[], NetworkAddress[]> clientAdderssDictionary =
            new Dictionary<string[], NetworkAddress[]>();
        private readonly Dictionary<string[], NetworkAddress[]> snppDictionary =
            new Dictionary<string[], NetworkAddress[]>();

        private void SendDirectoryAddressResponse(SignallingMessage message) {
            var clientAddress = clientAdderssDictionary[(string[])message.Payload];
            var directioryResponse = message;
            directioryResponse.Operation = SignallingMessageOperation.DirectoryAddressResponse;
            directioryResponse.Payload = clientAddress;
            SendMessage(directioryResponse);
        }

        private void SendDirectoryNameResponse(SignallingMessage message) {
            var clientName = clientAdderssDictionary.FirstOrDefault(x => x.Value == (NetworkAddress[])message.Payload).Key;
            var directioryResponse = message;
            directioryResponse.Operation = SignallingMessageOperation.DirectoryNameResponse;
            directioryResponse.Payload = clientName;
            SendMessage(directioryResponse);
        }

        private void SendDirectorySNPPResponse(SignallingMessage message) {
            var snpp = snppDictionary[(string[])message.Payload];
            var directioryResponse = message;
            directioryResponse.Operation = SignallingMessageOperation.DirectorySnppResponse;
            directioryResponse.Payload = snpp;
            SendMessage(directioryResponse);
        }

        public void UpdateDierctory() {
            //TO DO
        }

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.DirectoryAddressRequest:
                    SendDirectoryAddressResponse(message);
                    break;
                case SignallingMessageOperation.DirectoryNameRequest:
                    SendDirectoryNameResponse(message);
                    break;
                case SignallingMessageOperation.DirectorySnppRequest:
                    SendDirectoryNameResponse(message);
                    break;
            }
        }
    }
}