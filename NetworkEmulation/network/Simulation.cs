using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NetworkEmulation.Editor;
using NetworkEmulation.Editor.Element;
using NetworkEmulation.Properties;
using NetworkUtilities;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.Log;

namespace NetworkEmulation.Network {
    public class Simulation {
        private readonly CableCloud _cableCloud;

        private readonly List<NodeView> _initializableNodes;
        private readonly List<LinkView> _links;
        private readonly NameServer _nameServer;
        private readonly NetworkManagmentSystem _networkManagmentSystem;
        private readonly List<PathComputationServer> _pathComputationServers;

        private readonly Dictionary<int, Process> _processes;

        public Simulation(TreeNodeCollection networkHierarchy, List<LinkView> links) {
            _cableCloud = new CableCloud(Settings.Default.CableCloudUdpListenerPortNumber);
            PreapareCableCloudLogForm();
            _cableCloud.StartListening();

            _networkManagmentSystem = new NetworkManagmentSystem(Settings.Default.NetworkManagmentSystemListeningPort);
            PrepareNetworkManagmentSystemLogForm();
            _networkManagmentSystem.StartListening();

            _nameServer = new NameServer(Settings.Default.NameServerListeningPort);
            PrepareNameServerLogForm();
            _nameServer.StartListening();

            _pathComputationServers = new List<PathComputationServer>();
            _initializableNodes = new List<NodeView>();

            GetNetworkComponentsFromTree(networkHierarchy);

            PreparePathComputationServerMultipleLogForm();

            foreach (var pathComputationServer in _pathComputationServers) pathComputationServer.StartListening();

            foreach (var pathComputationServer in _pathComputationServers) pathComputationServer.Initialize();


            foreach (var initializableNode in _initializableNodes.OfType<NetworkNodeView>()) {
                initializableNode.DoubleClick += InitializableNodeOnDoubleClick;
                initializableNode.Parameters.MaxAtmCellsNumberInCableCloudMessage =
                    Settings.Default.MaxAtmCellsNumberInCableCloudMessage;
            }

            foreach (var initializableNode in _initializableNodes.OfType<ClientNodeView>()) {
                initializableNode.Parameters.MaxAtmCellsNumberInCableCloudMessage =
                    Settings.Default.MaxAtmCellsNumberInCableCloudMessage;
            }

            _links = links;

            _processes = new Dictionary<int, Process>();

            Run();
        }

        public bool Running { get; private set; }

        public LogForm CableCloudLogForm { get; private set; }
        public LogForm NetworkManagmentSystemLogForm { get; private set; }
        public LogForm NameServerLogForm { get; private set; }
        public PathComputationServerLogForm PathComputationServerLogForm { get; private set; }

        private void GetNetworkComponentsFromTree(TreeNodeCollection nodes) {
            foreach (TreeNode n in nodes) PutNetworkComponentInSuitableList(n);
        }

        private void PutNetworkComponentInSuitableList(TreeNode treeNode) {
            var component = treeNode.Tag;

            if (component is PathComputationServer) _pathComputationServers.Add((PathComputationServer) component);
            if (component is NodeView) {
                var pcs = treeNode.Parent.Tag as PathComputationServer;
                var node = (NodeView) component;

                if (node is ClientNodeView) {
                    var parameters = ((ClientNodeView) node).Parameters;
                    parameters.PathComputationServerListeningPort = pcs.ListeningPort;
                }

                if (node is NetworkNodeView) {
                    var parameters = ((NetworkNodeView) node).Parameters;
                    parameters.PathComputationServerListeningPort = pcs.ListeningPort;
                }

                _initializableNodes.Add(node);
            }

            foreach (TreeNode tn in treeNode.Nodes) PutNetworkComponentInSuitableList(tn);
        }

        private void PreapareCableCloudLogForm() {
            CableCloudLogForm = new LogForm(_cableCloud);
            CableCloudLogForm.Text = "Cable Cloud Log";
            CableCloudLogForm.Show();
        }

        private void PrepareNetworkManagmentSystemLogForm() {
            NetworkManagmentSystemLogForm = new LogForm(_networkManagmentSystem);
            NetworkManagmentSystemLogForm.Text = "Network Managment System Log";
            NetworkManagmentSystemLogForm.Show();
        }

        private void PrepareNameServerLogForm() {
            NameServerLogForm = new LogForm(_nameServer);
            NameServerLogForm.Text = "Name Server Log";
            NameServerLogForm.Show();
        }

        private void PreparePathComputationServerMultipleLogForm() {
            PathComputationServerLogForm = new PathComputationServerLogForm(_pathComputationServers);
            PathComputationServerLogForm.Show();
        }

        private void InitializableNodeOnDoubleClick(object sender, EventArgs eventArgs) {
            var networkNodeView = sender as NetworkNodeView;
            var nodeAddress = networkNodeView.NetworkAddress;
            var cableCloudDataPort = networkNodeView.CableCloudDataPort;

            if (_networkManagmentSystem.IsNetworkNodeOnline(nodeAddress)) {
                MarkAsOffline(networkNodeView);
                //TODO Mark links as offline

                KillProcess(cableCloudDataPort);
            }
            else {
                MarkAsOnline(networkNodeView);
                //TODO Mark links as online

                StartProcess(cableCloudDataPort);
            }
        }

        private void InitializeCableCloud() {
            CableCloudMessage.MaxAtmCellsNumber = Settings.Default.MaxAtmCellsNumberInCableCloudMessage;

            foreach (var link in _links) _cableCloud.AddLink(link);
        }

        private void MarkAsOnline(IMarkable markable) {
            markable.MarkAsOnline();
        }

        private void MarkAsOffline(IMarkable markable) {
            markable.MarkAsOffline();
        }

        private void MarkAsOnline(List<IMarkable> markables) {
            foreach (var markable in markables) markable.MarkAsOnline();
        }

        private void MarkAsOffline(List<IMarkable> markables) {
            foreach (var markable in markables) markable.MarkAsOffline();
        }

        private void MarkAsSelected(List<IMarkable> markables) {
            foreach (var markable in markables) markable.MarkAsSelected();
        }

        private void MarkAsDeselected(List<IMarkable> markables) {
            foreach (var markable in markables) markable.MarkAsDeselected();
        }

        private void InitializeNetworkManagmentSystem() {
            //WaitForNetworkNodesOnline();
            MarkAsOnline(_initializableNodes.OfType<IMarkable>().ToList());
            //TODO Mark links as online
        }

        private void WaitForNetworkNodesOnline() {
            while (!NetworkNodesOnline())
                Thread.Sleep(10);
        }

        private bool NetworkNodesOnline() {
            return _networkManagmentSystem.AreOnline(_initializableNodes.OfType<NetworkNodeView>().ToList());
        }

        public void Run() {
            if (!Running) {
                InitializeElements();
                StartProcesses();
                InitializeCableCloud();
                InitializeNetworkManagmentSystem();
                Running = true;
            }
        }

        private void InitializeElements() {
            foreach (var element in _initializableNodes) {
                var process = element.Initialize();

                if (process != null)
                    _processes.Add(element.CableCloudDataPort, process);
            }
        }

        private void StartProcesses() {
            foreach (var process in _processes) StartProcess(process.Key);
        }

        private void StartProcess(int id) {
            Thread.Sleep(100);
            var process = _processes[id];
            process.Start();
        }

        public void Stop() {
            MarkAsDeselected(_initializableNodes.OfType<IMarkable>().ToList());
            MarkAsDeselected(_links.OfType<IMarkable>().ToList());

            _cableCloud.Dispose();
            _networkManagmentSystem.Dispose();
            _nameServer.Dispose();

            foreach (var pathComputationServer in _pathComputationServers) pathComputationServer.Dispose();

            KillProcesses();

            Running = false;
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