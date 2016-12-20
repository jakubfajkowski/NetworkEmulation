using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NetworkEmulation.editor;
using NetworkEmulation.editor.element;
using NetworkEmulation.log;

namespace NetworkEmulation.network {
    public class Simulation {
        public LogForm CableCloudLogForm { get; }
        public LogForm NetworkManagmentSystemLogForm { get; }

        private readonly CableCloud _cableCloud;
        private readonly NetworkManagmentSystem _networkManagmentSystem;

        private readonly List<NodePictureBox> _initializableNodes;
        private readonly List<Link> _links;
        private readonly List<Connection> _connections;

        private readonly List<Process> _processes;

        public Simulation(List<NodePictureBox> initializableNodes, List<Link> links, List<Connection> connections) {
            _cableCloud = new CableCloud();
            CableCloudLogForm = new LogForm(_cableCloud);

            _networkManagmentSystem = new NetworkManagmentSystem();
            NetworkManagmentSystemLogForm = new LogForm(_networkManagmentSystem);

            _initializableNodes = initializableNodes;

            foreach (var initializableNode in _initializableNodes.OfType<NetworkNodePictureBox>()) {
                initializableNode.DoubleClick += InitializableNodeOnDoubleClick;
            }

            _links = links;
            _connections = connections;

            _processes = new List<Process>();
        }

        private void InitializableNodeOnDoubleClick(object sender, EventArgs eventArgs) {
            var networkNodePictureBox = sender as NetworkNodePictureBox;
            var nodeUdpPort = networkNodePictureBox.Parameters.NetworkManagmentSystemDataPort;

            if (_networkManagmentSystem.IsOnline(nodeUdpPort)) {
                _networkManagmentSystem.SendShutdownMessage(nodeUdpPort);
                MarkAsOffline(networkNodePictureBox);
            }
            else {
                _networkManagmentSystem.SendStartMessage(nodeUdpPort);
                MarkAsOnline(networkNodePictureBox);
            }
        }

        private void InitializeCableCloud() {
            foreach (var link in _links) {
                _cableCloud.AddLink(link);
            }
        }

        private void MarkAsOnline(IMarkable markable) {
            markable.MarkAsOnline();
        }

        private void MarkAsOffline(IMarkable markable) {
            markable.MarkAsOffline();
        }
        private void MarkAsOnline(List<IMarkable> markables) {
            foreach (var markable in markables) {
                markable.MarkAsOnline();
            }
        }

        private void MarkAsOffline(List<IMarkable> markables) {
            foreach (var markable in markables) {
                markable.MarkAsOffline();
            }
        }

        private void InitializeNetworkManagmentSystem() {
            _networkManagmentSystem.AreOnline(_initializableNodes.OfType<NetworkNodePictureBox>().ToList());
            MarkAsOnline(_initializableNodes.OfType<IMarkable>().ToList());

            foreach (var connection in _connections) {
                _networkManagmentSystem.SendConnectionToNetworkNodeAgent(connection);
            }
        }

        public void Run() {
            InitializeElements();
            StartProcesses();
            InitializeCableCloud();
            InitializeNetworkManagmentSystem();
        }

        private void InitializeElements() {
            foreach (var element in _initializableNodes) {
                var process = element.Initialize();

                if (process != null)
                    _processes.Add(process);
            }
        }

        private void StartProcesses() {
            foreach (var process in _processes) process.Start();
        }

        public void Stop() {
            _cableCloud.Dispose();
            _networkManagmentSystem.Dispose();
            KillProcesses();
        }

        private void KillProcesses() {
            foreach (var process in _processes) if(process.HasExited == false) process.Kill();
        }
    }
}