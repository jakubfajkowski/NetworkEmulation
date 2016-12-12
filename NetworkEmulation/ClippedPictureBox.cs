using System.Drawing;
using System.Windows.Forms;

namespace NetworkEmulation {
    public class ClippedPictureBox : PictureBox {
        public ClippedPictureBox() {
            SizeMode = PictureBoxSizeMode.AutoSize;
        }

        public new Image Image {
            get { return base.Image; }
            set {
                base.Image = value;
                clipRegion();
            }
        }

        private void clipRegion() {
            var _img = (Bitmap) Image;
            var rgn = new Region();
            rgn.MakeEmpty();
            var rc = new Rectangle(0, 0, 0, 0);
            var inimage = false;
            for (var y = 0; y < _img.Height; y++) {
                for (var x = 0; x < _img.Width; x++)
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
                if (inimage) {
                    inimage = false;
                    rc.Width = _img.Width - rc.X;
                    rgn.Union(rc);
                }
            }

            Region = rgn;
        }
    }
}