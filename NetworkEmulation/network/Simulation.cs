using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NetworkEmulation.editor;
using NetworkEmulation.editor.element;

namespace NetworkEmulation.network {
    public class Simulation {
        public CableCloud CableCloud { get; private set; }
        public NetworkManagmentSystem NetworkManagmentSystem { get; private set; }

        private readonly List<NodePictureBox> _initializableElements;
        private readonly List<Link> _links;
        private readonly List<Connection> _connections;

        private readonly List<Process> _processes;

        public Simulation(List<NodePictureBox> initializableElements, List<Link> links, List<Connection> connections) {
            CableCloud = new CableCloud();
            NetworkManagmentSystem = new NetworkManagmentSystem();

            _initializableElements = initializableElements;
            _links = links;
            _connections = connections;

            _processes = new List<Process>();

            InitializeCableCloud();
            InitializeNetworkManagmentSystem();
        }

        private void InitializeCableCloud() {
            foreach (var link in _links) {
                CableCloud.AddLink(link);
            }
        }

        private void InitializeNetworkManagmentSystem() {
            foreach (var connection in _connections) {
                NetworkManagmentSystem.SendConnectionToNetworkNodeAgent(connection);
            }
        }

        public void Run() {
            InitializeElements();
            StartProcesses();
        }

        private void InitializeElements() {
            foreach (var element in _initializableElements) {
                var process = element.Initialize();

                if (process != null)
                    _processes.Add(process);
            }
        }

        private void StartProcesses() {
            foreach (var process in _processes) process.Start();
        }

        public void Stop() {
            KillProcesses();
        }

        private void KillProcesses() {
            foreach (var process in _processes) process.Kill();
        }
    }
}