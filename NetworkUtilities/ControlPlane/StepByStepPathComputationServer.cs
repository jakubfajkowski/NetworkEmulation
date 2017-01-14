using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkUtilities.ControlPlane {
    class StepByStepPathComputationServer : ConnectionManager {
        private ConnectionController connectionController;
        private RoutingController routingController;
        private NetworkCallController networkCallController;

        public StepByStepPathComputationServer() {
            //TODO Bind send message handler.
        }

        protected override Task Listen(TcpClient nodeTcpClient, int port) {
            throw new NotImplementedException();
        }

        private void HandleMessage(object sender, SignallingMessage message) {
            //TODO: Message handling.
        }
    }
}
