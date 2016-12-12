using System.Drawing;
using System.Windows.Forms;

namespace NetworkEmulation.editor {
    public partial class Link : Control {
        private readonly NodePictureBox _beginNodePictureBox;
        private readonly Color _color = Color.Black;
        private readonly NodePictureBox _endNodePictureBox;
        private readonly Pen _pen;
        private readonly int _thickness = 3;

        public Link(ref NodePictureBox beginNodePictureBox, ref NodePictureBox endNodePictureBox) {
            //InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            _pen = new Pen(_color, _thickness);

            _beginNodePictureBox = beginNodePictureBox;
            _endNodePictureBox = endNodePictureBox;

            _beginNodePictureBox.OnNodeMoving += sender => Parent.Refresh();
            _endNodePictureBox.OnNodeMoving += sender => Parent.Refresh();
        }

        public void DrawLink(Graphics graphics) {
            var beginPoint = _beginNodePictureBox.CenterPoint();
            var endPoint = _endNodePictureBox.CenterPoint();


            graphics.DrawLine(_pen, beginPoint, endPoint);
        }
    }
}