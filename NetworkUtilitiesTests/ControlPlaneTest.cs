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

            var cpccBAddress = new NetworkAddress("2.2");
            CallingPartyCallController cpccB = new CallingPartyCallController(cpccBAddress);
            _controlPlaneElements.Add(cpccBAddress, cpccB);
            directory.UpdateDierctory("Babacki", cpccBAddress, new NetworkAddress("2.2.2"));

            var ncc1Address = new NetworkAddress("1");
            NetworkCallController ncc1 = new NetworkCallController(ncc1Address);
            _controlPlaneElements.Add(ncc1Address, ncc1);

            var ncc2Address = new NetworkAddress("2");
            NetworkCallController ncc2 = new NetworkCallController(ncc2Address);
            _controlPlaneElements.Add(ncc2Address, ncc2);

            ncc1.OnMessageToSend += PassMessage;
            ncc2.OnMessageToSend += PassMessage;
            cpccA.OnMessageToSend += PassMessage;
            cpccB.OnMessageToSend += PassMessage;
            directory.OnMessageToSend += PassMessage;

            cpccA.SendCallRequest("Abacki", "Babacki", ncc1Address);
        }

        private void PassMessage(object sender, SignallingMessage message) {
            var destination = _controlPlaneElements[message.DestinationAddress];
            destination.ReceiveMessage(message);
        }
    }
}
