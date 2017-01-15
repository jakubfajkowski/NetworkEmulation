using System.Collections.Generic;
using System.Linq;

namespace NetworkUtilities.ControlPlane {
    internal class Directory : ControlPlaneElement {
        private readonly Dictionary<string[], NetworkAddress[]> clientAdderssDictionary =
            new Dictionary<string[], NetworkAddress[]>();

        private void DirectoryResponse(string[] clientName) {
            //var clientAddress = clientAdderssDictionary[clientName];

            //var directioryResponse = new SignallingMessage(SignallingMessageOperation.DirectoryResponse, clientAddress);
            //SendMessage(directioryResponse);
        }

        private void DirectoryResponse(NetworkAddress[] clientAddress) {
            //var clientName = clientAdderssDictionary.FirstOrDefault(x => x.Value == clientAddress).Key;

            //var directioryResponse = new SignallingMessage(SignallingMessageOperation.DirectoryResponse, clientName);
            //SendMessage(directioryResponse);
        }

        public void UpdateDierctory() {
        }

        public override void RecieveMessage(SignallingMessage message) {
            switch (message.Operation) {
                case SignallingMessageOperation.DirectoryRequest:
                    if (message.Payload is string[]) {
                        DirectoryResponse((string[]) message.Payload);
                    } else if (message.Payload is NetworkAddress[]) {
                        DirectoryResponse((NetworkAddress[]) message.Payload);
                    }
                    break;
            }
        }
    }
}