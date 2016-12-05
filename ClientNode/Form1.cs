using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientNode {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClientNode client = new ClientNode();
            client.message = textBoxMessage.Text;
            client.createATMCell(1, 1, client.message);

            client.readDataFromATMCells();

            textBoxReceived.Text = client.receivedMessage;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        // nowe
        private void textBoxMessage_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxClients_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
