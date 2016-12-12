using System.Windows.Forms;

namespace NetworkEmulation {
    public class EditorPanel : Panel {
        //TODO Move inserted elements from MainForm to here.
        public EditorPanel() {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);
        }
    }
}