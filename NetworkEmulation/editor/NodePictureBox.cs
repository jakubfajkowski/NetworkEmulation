using System.Drawing;
using System.Windows.Forms;

namespace NetworkEmulation.editor {
    public abstract class NodePictureBox : ClippedPictureBox {
        public delegate void NodeMovingHandler(object sender);

        private int _xPos;
        private int _yPos;

        public new Point Location {
            get { return base.Location; }
            set {
                var imageCenter = new Size(Image.Size.Width/2, Image.Size.Height/2);
                base.Location = value - imageCenter;
            }
        }

        public event NodeMovingHandler OnNodeMoving;

        protected void NodeMoving() {
            OnNodeMoving?.Invoke(this);
        }

        public Point CenterPoint() {
            var imageCenter = new Size(Image.Size.Width/2, Image.Size.Height/2);
            return Location + imageCenter;
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                _xPos = e.X;
                _yPos = e.Y;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Top += e.Y - _yPos;
                Left += e.X - _xPos;

                NodeMoving();
            }
        }

        public abstract void MarkAsSelected();
        public abstract void MarkAsDeselected();
        public abstract void MarkAsOnline();
        public abstract void MarkAsOffline();

    }
}