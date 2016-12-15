using System.Collections.Generic;
using System.Diagnostics;
using NetworkEmulation.editor;

namespace NetworkEmulation.network {
    public class Simulation {
        private CableCloud _cableCloud;
        private List<IInitializable> _elements = new List<IInitializable>();
        private NetworkMangmentSystem _networkMangmentSystem;
        private List<Process> _processes = new List<Process>();

        public Simulation() {
            Prepare();
        }

        private void Prepare() {
            _cableCloud = new CableCloud();
            _networkMangmentSystem = new NetworkMangmentSystem();
            _elements = new List<IInitializable>();
            _processes = new List<Process>();
        }

        public void Run() {
            InitializeElements();
            StartProcesses();
        }

        private void InitializeElements() {
            foreach (var element in _elements) {
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