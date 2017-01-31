using System;
using System.Timers;
using NetworkUtilities.Log;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;

namespace NetworkUtilities.ManagementPlane {
    public class NetworkNodeAgent : LogObject {
        private readonly ConnectionComponent _managmentPlaneConnectionComponent;

        public NetworkNodeAgent(NetworkAddress networkAddress, string networkManagmentSystemIpAddress,
            int networkManagmentSystemListeningPort) {
            _managmentPlaneConnectionComponent = new ConnectionComponent(networkAddress, networkManagmentSystemIpAddress,
                networkManagmentSystemListeningPort, ConnectionManagerType.NetworkManagementSystem);
            _managmentPlaneConnectionComponent.UpdateState += (sender, state) => OnUpdateState(state);
        }

        public void Initialize() {
            _managmentPlaneConnectionComponent.Initialize();
            StartSendingKeepAliveMessages();
        }

        private void StartSendingKeepAliveMessages() {
            var timer = new Timer {
                AutoReset = true,
                Interval = 5000,
                Enabled = true
            };

            timer.Elapsed += OnTimedEvent;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e) {
            if (_managmentPlaneConnectionComponent.Online) {
                var message = new ManagementMessage(ManagementMessageType.KeepAlive, new object());
                _managmentPlaneConnectionComponent.Send(message);
                //OnUpdateState("[SENT] [KEEP_ALIVE]");
            }
        }
    }
}