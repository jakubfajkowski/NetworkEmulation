using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NetworkEmulation.Properties;
using NetworkEmulation.Workplace;
using NetworkEmulation.Workplace.Element;
using NetworkUtilities.ControlPlane;
using NetworkUtilities.DataPlane;
using NetworkUtilities.Log;
using NetworkUtilities.ManagementPlane;
using NetworkUtilities.Network;

namespace NetworkEmulation {
    public class Simulation : IDisposable {
        private readonly CableCloud _cableCloud;

        private readonly List<NodeView> _initializableNodes;
        private readonly List<LinkView> _links;
        private readonly NameServer _nameServer;
        private readonly NetworkManagementSystem _networkManagmentSystem;
        private readonly List<PathComputationServer> _pathComputationServers;

        private readonly Dictionary<int, Process> _processes;
        private readonly SignallingCloud _signallingCloud;

        public Simulation(TreeNodeCollection networkHierarchy, List<LinkView> links) {
            _cableCloud = new CableCloud(Settings.Default.CableCloudListenerPort);
            _signallingCloud = new SignallingCloud(Settings.Default.SignallingCloudListeningPort);
            _networkManagmentSystem = new NetworkManagementSystem(Settings.Default.NetworkManagmentSystemListeningPort);


            var connectionManagers = new List<LogObject>(new LogObject[]{_cableCloud, _signallingCloud, _networkManagmentSystem});
            ConnectionManagersLogForm = PrepareMultiLogForm(connectionManagers, "Connection Managers Log");

            _cableCloud.StartListening();
            _signallingCloud.StartListening();
            _networkManagmentSystem.StartListening();

            _nameServer = new NameServer(Settings.Default.IpAddress, Settings.Default.SignallingCloudListeningPort);
            PrepareNameServerLogForm();
            _nameServer.Initialize();

            _pathComputationServers = new List<PathComputationServer>();
            _initializableNodes = new List<NodeView>();

            GetNetworkComponentsFromTree(networkHierarchy);

            PathComputationServersLogForm = PrepareMultiLogForm(_pathComputationServers.Cast<LogObject>().ToList(), "Path Computation Servers Log");

            foreach (var pathComputationServer in _pathComputationServers) {
                pathComputationServer.Initialize();
            }

            _links = links;

            foreach (var initializableNode in _initializableNodes.OfType<NetworkNodeView>()) {
                if (!initializableNode.DoubleClickEnabled) {
                    initializableNode.DoubleClick += InitializableNodeOnDoubleClick;
                    initializableNode.DoubleClickEnabled = true;
                }
                initializableNode.Parameters.MaxAtmCellsNumberInCableCloudMessage =
                    Settings.Default.MaxAtmCellsNumberInCableCloudMessage;
            }

            foreach (var initializableNode in _initializableNodes.OfType<ClientNodeView>()) {
                initializableNode.Parameters.MaxAtmCellsNumberInCableCloudMessage =
                    Settings.Default.MaxAtmCellsNumberInCableCloudMessage;
                _nameServer.UpdateDirectory(initializableNode.Parameters.ClientName,
                    new SubnetworkPointPool(initializableNode.NetworkAddress.Append(1)));
            }

            _processes = new Dictionary<int, Process>();

            Run();
        }

        public bool Running { get; private set; }

        public MultiLogForm ConnectionManagersLogForm { get; private set; }
        public LogForm NameServerLogForm { get; private set; }
        public MultiLogForm PathComputationServersLogForm { get; private set; }

        private void GetNetworkComponentsFromTree(TreeNodeCollection nodes) {
            foreach (TreeNode n in nodes) PutNetworkComponentInSuitableList(n);
        }

        private void PutNetworkComponentInSuitableList(TreeNode treeNode) {
            var component = treeNode.Tag;

            if (component is PathComputationServer) _pathComputationServers.Add((PathComputationServer) component);
            if (component is NodeView) {
                var node = (NodeView) component;

                _initializableNodes.Add(node);
            }

            foreach (TreeNode tn in treeNode.Nodes) PutNetworkComponentInSuitableList(tn);
        }



        private MultiLogForm PrepareMultiLogForm(List<LogObject> logObjects, string title) {
            var multiLogForm = new MultiLogForm(logObjects);
            multiLogForm.Text = title;
            multiLogForm.Show();

            return multiLogForm;
        }

        private void PrepareNameServerLogForm() {
            NameServerLogForm = new LogForm(_nameServer);
            NameServerLogForm.Text = "Name Server Log";
            NameServerLogForm.Show();
        }

        private void InitializableNodeOnDoubleClick(object sender, EventArgs eventArgs) {
            var networkNodeView = sender as NetworkNodeView;
            var nodeAddress = networkNodeView.NetworkAddress;
            var cableCloudDataPort = networkNodeView.CableCloudDataPort;

            if (_networkManagmentSystem.IsConnected(nodeAddress)) {
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

            foreach (var link in _links) {
                _cableCloud.AddLink(link.Parameters.InputNodePortPair, link.Parameters.OutputNodePortPair);
                _cableCloud.AddLink(link.Parameters.OutputNodePortPair, link.Parameters.InputNodePortPair);
            }
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
            WaitForNetworkNodesConnected();
            MarkAsOnline(_initializableNodes.OfType<IMarkable>().ToList());

            foreach (var linkView in _links) {
                if (!(linkView.BeginNodeView is ClientNodeView) && !(linkView.EndNodeView is ClientNodeView)) {
                    var link = new Link(linkView.Parameters);
                    _networkManagmentSystem.SendConfigurationMessage(link);
                    _networkManagmentSystem.SendConfigurationMessage(link.Reverse());
                }
                linkView.MarkAsOnline();
            }
        }

        private void WaitForNetworkNodesConnected() {
            while (!NetworkNodesOnline())
                Thread.Sleep(100);
        }

        private bool NetworkNodesOnline() {
            var nodes = _initializableNodes.OfType<NetworkNodeView>().Select(view => view.NetworkAddress).ToList();
            return _networkManagmentSystem.AreConnected(nodes) && _signallingCloud.AreConnected(nodes);
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

            Dispose();

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

        public void Dispose() {
            _cableCloud?.Dispose();
            _nameServer?.Dispose();
            _networkManagmentSystem?.Dispose();
            _signallingCloud?.Dispose();
            foreach (var pathComputationServer in _pathComputationServers) {
                pathComputationServer?.Dispose();
            }
        }
    }
}