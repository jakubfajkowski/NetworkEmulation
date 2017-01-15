using System.Collections.Generic;
using System.Linq;

namespace NetworkUtilities.ControlPlane {
    internal class Directory : ControlPlaneElement {
        private readonly Dictionary<string[], NetworkAddress[]> clientAdderssDictionary =
            new Dictionary<string[], NetworkAddress[]>();

        private void SendDirectoryResponseAddress(SignallingMessage message) {
            var clientAddress = clientAdderssDictionary[(string[])message.Payload];
            var directioryResponse = message;
            directioryResponse.Operation = SignallingMessageOperation.DirectoryResponse;
            directioryResponse.Payload = clientAddress;
            SendMessage(directioryResponse);
        }

        private void SendDirectoryResponseName(SignallingMessage message) {
            var clientName = clientAdderssDictionary.FirstOrDefault(x => x.Value == (NetworkAddress[])message.Payload).Key;
            var directioryResponse = message;
            directioryResponse.Operation = SignallingMessageOperation.DirectoryResponse;
            directioryResponse.Payload = clientName;
            SendMessage(directioryResponse);
        }

        private void SendDirectoryResponseSNPP(SignallingMessage message) {
            
        }

        public void UpdateDierctory() {
        }

        public override void ReceiveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.DirectoryRequest:
                    if (message.Payload is string[]) {
                        SendDirectoryResponseAddress(message);
                    } else if (message.Payload is NetworkAddress[]) {
                        SendDirectoryResponseName(message);
                    }
                    break;
            }
        }
    }
}