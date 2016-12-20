using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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

        private readonly Dictionary<int, Process> _processes;

        public Simulation(List<NodePictureBox> initializableNodes, List<Link> links, List<Connection> connections) {
            _cableCloud = new CableCloud();
            CableCloudLogForm = new LogForm(_cableCloud);
            _cableCloud.OnUpdateState += (sender, state) => Console.WriteLine(state);

            _networkManagmentSystem = new NetworkManagmentSystem();
            NetworkManagmentSystemLogForm = new LogForm(_networkManagmentSystem);
            _networkManagmentSystem.OnUpdateState += (sender, state) => Console.WriteLine(state);

            _initializableNodes = initializableNodes;

            foreach (var initializableNode in _initializableNodes.OfType<NetworkNodePictureBox>()) {
                initializableNode.DoubleClick += InitializableNodeOnDoubleClick;
            }

            _links = links;
            _connections = connections;

            _processes = new Dictionary<int, Process>();
        }

        private void InitializableNodeOnDoubleClick(object sender, EventArgs eventArgs) {
            var networkNodePictureBox = sender as NetworkNodePictureBox;
            var nodeUdpPort = networkNodePictureBox.Parameters.NetworkManagmentSystemDataPort;
            var cableCloudDataPort = networkNodePictureBox.CableCloudDataPort;

            if (_networkManagmentSystem.IsOnline(nodeUdpPort)) {
                KillProcess(cableCloudDataPort);
                MarkAsOffline(networkNodePictureBox);
            }
            else {
                StartProcess(cableCloudDataPort);
                MarkAsOnline(networkNodePictureBox);
                InitializeNetworkManagmentSystem(networkNodePictureBox);
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
        private void MarkAsSelected(List<IMarkable> markables) {
            foreach (var markable in markables) {
                markable.MarkAsSelected();
            }
        }

        private void MarkAsDeselected(List<IMarkable> markables) {
            foreach (var markable in markables) {
                markable.MarkAsDeselected();
            }
        }

        private void InitializeNetworkManagmentSystem() {
            WaitForNetworkNodesOnline();
            MarkAsOnline(_initializableNodes.OfType<IMarkable>().ToList());

            foreach (var connection in _connections) {
                _networkManagmentSystem.SendConnectionToNetworkNodeAgent(connection);
            }
        }

        private void InitializeNetworkManagmentSystem(NetworkNodePictureBox networkNodePictureBox) {
            WaitForNetworkNodesOnline();
            var nodeConnectionInformations = new List<NodeConnectionInformation>();
            var nodeUdpPort = networkNodePictureBox.Parameters.NetworkManagmentSystemDataPort;

            foreach (var connection in _connections) {
                nodeConnectionInformations.AddRange(connection.Parameters.NodeConnectionInformations.FindAll(
                    information => information.NodeUdpPort == nodeUdpPort));
            }

            foreach (var nodeConnectionInformation in nodeConnectionInformations) {
                _networkManagmentSystem.SendConnectionToNetworkNodeAgent(nodeConnectionInformation);
            }
        }

        private void WaitForNetworkNodesOnline() {
            while (!NetworkNodesOnline())
                Thread.Sleep(10);
        }

        private bool NetworkNodesOnline() {
            return _networkManagmentSystem.AreOnline(_initializableNodes.OfType<NetworkNodePictureBox>().ToList());
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
                    _processes.Add(element.CableCloudDataPort, process);
            }
        }

        private void StartProcesses() {
            foreach (var process in _processes) {
                StartProcess(process.Key);
            }
        }

        private void StartProcess(int id) {
            var process = _processes[id];
                process.Start();
        }

        public void Stop() {
            MarkAsDeselected(_initializableNodes.OfType<IMarkable>().ToList());
            MarkAsDeselected(_links.OfType<IMarkable>().ToList());
            MarkAsSelected(_connections.OfType<IMarkable>().ToList());

            _cableCloud.Dispose();
            _networkManagmentSystem.Dispose();
            KillProcesses();
        }

        private void KillProcesses() {
            foreach (var process in _processes)
                KillProcess(process.Key);
        }

        private void KillProcess(int id) {
            var process = _processes[id];

            if (process.HasExited == false)
                process.Kill();
        }
    }
}