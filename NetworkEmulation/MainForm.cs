using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NetworkEmulation.Properties;

namespace NetworkEmulation {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void editorPanel_MouseClick(object sender, MouseEventArgs e) {
            Bitmap bitmap = Resources.ClientNode;
            bitmap.MakeTransparent();

            MovablePictureBox pictureBox = new MovablePictureBox();
            pictureBox.Image = bitmap;
            pictureBox.Location = MousePosition;
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            editorPanel.Controls.Add(pictureBox);
        }

        private void addClientNodeMenuItem_Click(object sender, EventArgs e) {
            Bitmap b = Resources.ClientNode;

            Graphics g = Graphics.FromImage(b);
            IntPtr ptr = b.GetHicon();
            Cursor c = new Cursor(ptr);
            this.editorPanel.Cursor = c;
        }

        private void addNetworkNodeMenuItem_Click(object sender, EventArgs e) {
            Bitmap b = Resources.NetworkNode;

            Graphics g = Graphics.FromImage(b);
            IntPtr ptr = b.GetHicon();
            Cursor c = new Cursor(ptr);
            this.editorPanel.Cursor = c;
        }

        private void addLinkMenuItem_Click(object sender, EventArgs e) {

        }

        private void addConnectionMenuItem_Click(object sender, EventArgs e) {

        }
    }
}
