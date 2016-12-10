using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkEmulation {
    class ClippedPictureBox : PictureBox {
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
}
