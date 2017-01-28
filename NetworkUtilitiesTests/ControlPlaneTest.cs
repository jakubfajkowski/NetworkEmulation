using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;

namespace NetworkUtilitiesTests {
    [TestClass]
    public class ControlPlaneTest {
        readonly Dictionary<NetworkAddress, ControlPlaneElement> _controlPlaneElements =
            new Dictionary<NetworkAddress, ControlPlaneElement>();

        [TestMethod]
        public void TestMessageTransfer() {

            Directory directory = new Directory(Directory.Address);
            _controlPlaneElements.Add(Directory.Address, directory);

            var cpccAAddress = new NetworkAddress("1.1");
            CallingPartyCallController cpccA = new CallingPartyCallController(cpccAAddress);
            _controlPlaneElements.Add(cpccAAddress, cpccA);
            directory.UpdateDierctory("Abacki", cpccAAddress, new NetworkAddress("1.1.1"));

            var cpccBAddress = new NetworkAddress("1.2");
            CallingPartyCallController cpccB = new CallingPartyCallController(cpccBAddress);
            _controlPlaneElements.Add(cpccBAddress, cpccB);
            directory.UpdateDierctory("Babacki", cpccBAddress, new NetworkAddress("1.2.2"));

            var nccAddress = new NetworkAddress("1");
            NetworkCallController ncc = new NetworkCallController(nccAddress);
            _controlPlaneElements.Add(nccAddress, ncc);

            ncc.OnMessageToSend += PassMessage;
            cpccA.OnMessageToSend += PassMessage;
            cpccB.OnMessageToSend += PassMessage;
            directory.OnMessageToSend += PassMessage;

            cpccA.SendCallRequest("Abacki", "Babacki", nccAddress);
        }

        private void PassMessage(object sender, SignallingMessage message) {
            var destination = _controlPlaneElements[message.DestinationAddress];
            destination.ReceiveMessage(message);
        }
    }
}
