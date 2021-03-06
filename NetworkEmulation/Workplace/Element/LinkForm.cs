﻿using System;
using System.Windows.Forms;
using NetworkUtilities.Utilities;

namespace NetworkEmulation.Workplace.Element {
    public partial class LinkForm : Form {
        private readonly LinkView _link;

        public LinkForm(LinkView link) {
            InitializeComponent();
            _link = link;
        }

        private void OkClick(object sender, EventArgs e) {
            var inputNodePort = 1;
            if (_link.BeginNodeView is NetworkNodeView) {
                inputNodePort = ((NetworkNodeView)_link.BeginNodeView).GetFreePort();
            }


            var outputNodePort = 1;
            if (_link.EndNodeView is NetworkNodeView) {
                outputNodePort = ((NetworkNodeView) _link.EndNodeView).GetFreePort();
            }

            var inputNetworkAddress = _link.BeginNodeView.NetworkAddress;
            var outputNetworkAddress = _link.EndNodeView.NetworkAddress;

            _link.Parameters.InputNodePortPair = new NetworkAddressNodePortPair(inputNetworkAddress, inputNodePort);
            _link.Parameters.OutputNodePortPair = new NetworkAddressNodePortPair(outputNetworkAddress, outputNodePort);

            _link.Parameters.Capacity = int.Parse(textBoxCapacity.Text);

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }
    }
}