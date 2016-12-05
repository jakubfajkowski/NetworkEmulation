using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

class MovablePictureBox : PictureBox {
    private int xPos;
    private int yPos;

    protected override void OnMouseDown(MouseEventArgs e) {
        if (e.Button == MouseButtons.Left) {
            xPos = e.X;
            yPos = e.Y;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e) {
        if (e.Button == MouseButtons.Left) {
            this.Top += (e.Y - yPos);
            this.Left += (e.X - xPos);
        }
    }
}