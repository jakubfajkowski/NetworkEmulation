﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private CableCloud _cableCloud;
        private NetworkManagmentSystem _networkManagmentSystem;

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
            _links = links;
            _connections = connections;

            _processes = new List<Process>();

            InitializeCableCloud();
            InitializeNetworkManagmentSystem();
        }

        private void InitializeCableCloud() {
            foreach (var link in _links) {
                _cableCloud.AddLink(link);
            }
        }

        private void InitializeNetworkManagmentSystem() {
            foreach (var connection in _connections) {
                _networkManagmentSystem.SendConnectionToNetworkNodeAgent(connection);
            }
        }

        public void Run() {
            InitializeElements();
            StartProcesses();
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
            KillProcesses();
        }

        private void KillProcesses() {
            foreach (var process in _processes) process.Kill();
        }
    }
}