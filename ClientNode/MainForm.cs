using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetworkUtilities;

namespace ClientNode {
    public partial class MainForm : Form {

        ClientNode client;
        List<ATMCell> atmCells;

        public MainForm() {
            InitializeComponent();
            client = new ClientNode(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            client.message = textBoxMessage.Text;
            atmCells = client.createATMCell(1, 1, client.message);
            
            //client.readDataFromATMCells(atmCells);
            //textBoxReceived.Text = client.receivedMessage;

            //string selected = comboBoxClients.SelectedItem.ToString();
            addClientToComboBox("hehe");
            client.addClient(1, 1, 1, "Adam");
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBoxMessage_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        public void addClientToComboBox(string clientName) {
            comboBoxClients.Items.Add(clientName);
        }

        private void comboBoxClients_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBoxEventLog_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
