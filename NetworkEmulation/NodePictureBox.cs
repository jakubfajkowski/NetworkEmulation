using System.Drawing;
using System.Windows.Forms;

namespace NetworkEmulation {
    class NodePictureBox : ClippedPictureBox {
        private int xPos;
        private int yPos;

        public new Point Location {
            get { return base.Location; }
            set {
                var imageCenter = new Size(Image.Size.Width/2, Image.Size.Height/2);
                base.Location = value - imageCenter;
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
    }
}