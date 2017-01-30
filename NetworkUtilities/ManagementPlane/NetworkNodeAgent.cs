using System.Timers;
using NetworkUtilities.Log;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities;
using Timer = System.Timers.Timer;

namespace NetworkUtilities.ManagementPlane {
    public class NetworkNodeAgent : LogObject {
        private readonly ConnectionComponent _managmentPlaneConnectionComponent;

        public NetworkNodeAgent(NetworkAddress networkAddress, string networkManagmentSystemIpAddress, int networkManagmentSystemListeningPort) {
            _managmentPlaneConnectionComponent = new ConnectionComponent(networkAddress, networkManagmentSystemIpAddress, networkManagmentSystemListeningPort);
        }

        public void Initialize() {
            _managmentPlaneConnectionComponent.Initialize();
            StartSendingKeepAliveMessages();
        }

        private void StartSendingKeepAliveMessages() {
            var timer = new Timer {
                AutoReset = true,
                Interval = 1000,
                Enabled = true
            };

            timer.Elapsed += OnTimedEvent;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e) {
            if (_managmentPlaneConnectionComponent.Online) {
                _managmentPlaneConnectionComponent.Send("KEEP_ALIVE");
                OnUpdateState("Keep alive sent");
            }
        }
    }
}