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

            Directory directory = new Directory();
            _controlPlaneElements.Add(Directory.Address, directory);

            CallingPartyCallController cpccA = new CallingPartyCallController();
            _controlPlaneElements.Add(new NetworkAddress("1.1"), cpccA);
            directory.UpdateDierctory("Abacki", new NetworkAddress("1.1"), new NetworkAddress("1.1.1"));

            CallingPartyCallController cpccB = new CallingPartyCallController();
            _controlPlaneElements.Add(new NetworkAddress("1.2"), cpccB);
            directory.UpdateDierctory("Babacki", new NetworkAddress("1.2"), new NetworkAddress("1.2.2"));

            NetworkCallController ncc = new NetworkCallController();
            _controlPlaneElements.Add(new NetworkAddress("1"), ncc);

            ncc.OnMessageToSend += PassMessage;
            cpccA.OnMessageToSend += PassMessage;
            cpccB.OnMessageToSend += PassMessage;
            directory.OnMessageToSend += PassMessage;

            cpccA.SendCallRequest("Abacki", "Babacki", new NetworkAddress("1"));
        }

        private void PassMessage(object sender, SignallingMessage message) {
            var destination = _controlPlaneElements[message.DestinationAddress];
            destination.ReceiveMessage(message);
        }
    }
}
