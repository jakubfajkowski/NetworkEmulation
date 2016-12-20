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

        private List<Process> _processes = new List<Process>();

        public Simulation(List<NodePictureBox> initializableElements, List<Link> links, List<Connection> connections) {
            _initializableElements = initializableElements;
            _links = links;
            _connections = connections;

            Prepare();
        }

        private void Prepare() {
            CableCloud = new CableCloud();
            NetworkManagmentSystem = new NetworkManagmentSystem();
            _processes = new List<Process>();
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
            Prepare();
        }

        private void KillProcesses() {
            foreach (var process in _processes) process.Kill();
        }
    }
}