using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NetworkEmulation.editor;

namespace NetworkEmulation.network {
    public class Simulation : IXmlSerializable {
        private CableCloud _cableCloud;
        private List<IInitializable> _elements = new List<IInitializable>();
        private NetworkManagmentSystem _networkMangmentSystem;
        private List<Process> _processes = new List<Process>();

        public Simulation() {
            Prepare();
        }

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer) {
            throw new NotImplementedException();
        }

        private void Prepare() {
            _cableCloud = new CableCloud();
            _networkMangmentSystem = new NetworkManagmentSystem();
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