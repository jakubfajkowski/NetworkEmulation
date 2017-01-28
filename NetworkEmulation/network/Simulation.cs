using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NetworkEmulation.Editor;
using NetworkEmulation.Editor.Element;
using NetworkEmulation.Properties;
using NetworkUtilities;
using NetworkUtilities.Log;

namespace NetworkEmulation.Network {
    public class Simulation {
        private readonly CableCloud _cableCloud;

        private readonly List<NodeView> _initializableNodes;
        private readonly List<LinkView> _links;
        private readonly NetworkManagmentSystem _networkManagmentSystem;

        private readonly Dictionary<int, Process> _processes;
        private bool _cableCloudLogFormShown;
        private bool _networkManagmentSystemLogFormShown;

        public Simulation(List<NodeView> initializableNodes, List<LinkView> links) {
            //TODO Zmienić metodę pokazywania logu
            _cableCloud = new CableCloud(Settings.Default.CableCloudUdpListenerPortNumber);
            CableCloudLogForm = new LogForm(_cableCloud);
            PreapareCableCloudLogForm();
            _cableCloud.Initialize();

            _networkManagmentSystem = new NetworkManagmentSystem();
            NetworkManagmentSystemLogForm = new LogForm(_networkManagmentSystem);
            PrepareNetworkManagmentSystemLogForm();

            _initializableNodes = initializableNodes;

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

        public bool Running { get; private set; }

        public LogForm CableCloudLogForm { get; }
        public LogForm NetworkManagmentSystemLogForm { get; }

        private void PreapareCableCloudLogForm() {
            CableCloudLogForm.Text = "Cable Cloud Log";
            CableCloudLogForm.Shown += CableCloudLogForm_Shown;
            CableCloudLogForm.Show();
        }

        private void PrepareNetworkManagmentSystemLogForm() {
            NetworkManagmentSystemLogForm.Text = "Network Managment System Log";
            NetworkManagmentSystemLogForm.Shown += NetworkManagmentSystemLogForm_Shown;
            NetworkManagmentSystemLogForm.Show();
        }

        private void CableCloudLogForm_Shown(object sender, EventArgs e) {
            _cableCloudLogFormShown = true;

            if (_networkManagmentSystemLogFormShown) Run();
        }

        private void NetworkManagmentSystemLogForm_Shown(object sender, EventArgs e) {
            _networkManagmentSystemLogFormShown = true;

            if (_cableCloudLogFormShown) Run();
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