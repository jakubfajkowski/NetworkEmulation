using System;
using System.Drawing;
using System.Windows.Forms;

namespace NetworkEmulation {
    class LinkPictureBox : ClippedPictureBox {
        // TODO Draw link on editorPanel instead of ClippedPictureBoxes, because of lack of performance when link is long and moving.
        private readonly int _thickness = 3;
        private readonly Color _color = Color.Black;
        private readonly Pen pen;

        private Point _beginPoint;
        private Point _endPoint;

        public LinkPictureBox(NodePictureBox beginNodePictureBox, NodePictureBox endNodePictureBox) {
            pen = new Pen(_color, _thickness);

            _beginPoint = beginNodePictureBox.Location;
            _endPoint = endNodePictureBox.Location;

            Image = CreateLinkDrawing();
            Location = FindCenter(_beginPoint, _endPoint);
        }

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

        private Point FindCenter(Point beginPoint, Point endPoint) {
            var x = beginPoint.X - (beginPoint.X + endPoint.X) / 2;
            var y = beginPoint.Y - (beginPoint.Y + endPoint.Y) / 2;

            return new Point(x, y);
        }

        public Bitmap CreateLinkDrawing() {
            var dX = _endPoint.X - _beginPoint.X;
            var dY = _endPoint.Y - _beginPoint.Y;

            var width = Math.Abs(dX);
            var height = Math.Abs(dY);

            var bitmap = new Bitmap(width + _thickness, height + _thickness);

            using (var graphics = Graphics.FromImage(bitmap)) {
                if (dX * dY < 0) {
                    graphics.DrawLine(pen, 0, height, width, 0);
                }
                else {
                    graphics.DrawLine(pen, width, height, 0, 0);
                }
            }

            return bitmap;
        }
    }
}