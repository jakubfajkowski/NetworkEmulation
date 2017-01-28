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
        private readonly NetworkManagmentSystem _networkManagmentSystem;
        private readonly List<PathComputationServer> _pathComputationServers;

        private readonly List<NodeView> _initializableNodes;
        private readonly List<LinkView> _links;

        private readonly Dictionary<int, Process> _processes;

        private bool _cableCloudLogFormShown;
        private bool _networkManagmentSystemLogFormShown;
        private bool _pathComputationServerLogFormShown;

        public Simulation(TreeNodeCollection networkHierarchy, List<LinkView> links) {
            //TODO Zmienić metodę pokazywania logu
            _cableCloud = new CableCloud(Settings.Default.CableCloudUdpListenerPortNumber);
            PreapareCableCloudLogForm();
            _cableCloud.Initialize();

            _networkManagmentSystem = new NetworkManagmentSystem();
            PrepareNetworkManagmentSystemLogForm();

            _pathComputationServers = new List<PathComputationServer>();
            _initializableNodes = new List<NodeView>();

            GetNetworkComponentsFromTree(networkHierarchy);

            PreparePathComputationServerMultipleLogForm();
            foreach (var pathComputationServer in _pathComputationServers) {
                pathComputationServer.Initialize();
            }


            foreach (var initializableNode in _initializableNodes.OfType<NetworkNodeView>()) {
                initializableNode.DoubleClick += InitializableNodeOnDoubleClick;
                initializableNode.Parameters.MaxAtmCellsNumberInCableCloudMessage =
                    Settings.Default.MaxAtmCellsNumberInCableCloudMessage;
            }

            foreach (var initializableNode in _initializableNodes.OfType<ClientNodeView>())
                initializableNode.Parameters.MaxAtmCellsNumberInCableCloudMessage =
                    Settings.Default.MaxAtmCellsNumberInCableCloudMessage;

            _links = links;

            _processes = new Dictionary<int, Process>();
        }

        private void GetNetworkComponentsFromTree(TreeNodeCollection nodes) {
            foreach (TreeNode n in nodes) {
                PutNetworkComponentInSuitableList(n);
            }
        }

        private void PutNetworkComponentInSuitableList(TreeNode treeNode) {
            foreach (TreeNode tn in treeNode.Nodes) {
                if (tn.Tag is PathComputationServer) _pathComputationServers.Add((PathComputationServer)tn.Tag);
                if (tn.Tag is NodeView) _initializableNodes.Add((NodeView)tn.Tag);

                PutNetworkComponentInSuitableList(tn);
            }
        }

        public bool Running { get; private set; }

        public LogForm CableCloudLogForm { get; private set; }
        public LogForm NetworkManagmentSystemLogForm { get; private set; }
        public PathComputationServerLogForm PathComputationServerLogForm { get; private set; }

        private void PreapareCableCloudLogForm() {
            CableCloudLogForm = new LogForm(_cableCloud);
            CableCloudLogForm.Text = "Cable Cloud Log";
            CableCloudLogForm.Shown += CableCloudLogForm_Shown;
            CableCloudLogForm.Show();
        }

        private void PrepareNetworkManagmentSystemLogForm() {
            NetworkManagmentSystemLogForm = new LogForm(_networkManagmentSystem);
            NetworkManagmentSystemLogForm.Text = "Network Managment System Log";
            NetworkManagmentSystemLogForm.Shown += NetworkManagmentSystemLogForm_Shown;
            NetworkManagmentSystemLogForm.Show();
        }

        private void PreparePathComputationServerMultipleLogForm() {
            PathComputationServerLogForm = new PathComputationServerLogForm(_pathComputationServers);
            PathComputationServerLogForm.Shown += PathComputationServerLogForm_Shown;
            PathComputationServerLogForm.Show();
        }

        private void CableCloudLogForm_Shown(object sender, EventArgs e) {
            _cableCloudLogFormShown = true;

            if (_networkManagmentSystemLogFormShown && _pathComputationServerLogFormShown) Run();
        }

        private void NetworkManagmentSystemLogForm_Shown(object sender, EventArgs e) {
            _networkManagmentSystemLogFormShown = true;

            if (_cableCloudLogFormShown && _pathComputationServerLogFormShown) Run();
        }

        private void PathComputationServerLogForm_Shown(object sender, EventArgs e) {
            _pathComputationServerLogFormShown = true;

            if (_cableCloudLogFormShown && _networkManagmentSystemLogFormShown) Run();
        }

        private void InitializableNodeOnDoubleClick(object sender, EventArgs eventArgs) {
            var networkNodeView = sender as NetworkNodeView;
            var nodeUdpPort = networkNodeView.Parameters.NetworkManagmentSystemDataPort;
            var cableCloudDataPort = networkNodeView.CableCloudDataPort;

            if (_networkManagmentSystem.IsOnline(nodeUdpPort)) {
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
            WaitForNetworkNodesOnline();
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
            var process = _processes[id];
            process.Start();
        }

        public void Stop() {
            MarkAsDeselected(_initializableNodes.OfType<IMarkable>().ToList());
            MarkAsDeselected(_links.OfType<IMarkable>().ToList());

            _cableCloud.Dispose();
            _networkManagmentSystem.Dispose();

            foreach (var pathComputationServer in _pathComputationServers) {
                pathComputationServer.Dispose();
            }
            
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