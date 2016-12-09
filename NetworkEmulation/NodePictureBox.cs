using System;
using System.Drawing;
using System.Windows.Forms;
using NetworkEmulation;

class NodePictureBox : PictureBox {
    private int xPos;
    private int yPos;

    public new Point Location {
        get { return base.Location; }
        set {
            var imageCenter = new Size(Image.Size.Width/2, Image.Size.Height/2);
            base.Location = value - imageCenter;
        }
    }
    public new Image Image {
        get {
            return base.Image;
        }
        set {
            base.Image = value;
            clipRegion();
        }
    }

    public NodePictureBox() {
        SizeMode = PictureBoxSizeMode.AutoSize;
    }

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

    private void clipRegion() {
        var _img = (Bitmap) Image;
        var rgn = new Region();
        rgn.MakeEmpty();
        var rc = new Rectangle(0, 0, 0, 0);
        bool inimage = false;
        for (int y = 0; y < _img.Height; y++) {
            for (int x = 0; x < _img.Width; x++) {
                if (!inimage) {
                    if (_img.GetPixel(x, y).A > 128) {
                        inimage = true;
                        rc.X = x;
                        rc.Y = y;
                        rc.Height = 1;
                    }
                }
                else {
                    if (_img.GetPixel(x, y).A <= 128) {
                        inimage = false;
                        rc.Width = x - rc.X;
                        rgn.Union(rc);
                    }
                }
            }
            if (inimage) {
                inimage = false;
                rc.Width = _img.Width - rc.X;
                rgn.Union(rc);
            }
        }

        Region = rgn;
    }
}