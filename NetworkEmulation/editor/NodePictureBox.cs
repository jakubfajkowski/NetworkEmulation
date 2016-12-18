using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NetworkEmulation.editor {
    public abstract class NodePictureBox : ClippedPictureBox, IMarkable, IInitializable, IXmlSerializable {
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

        public abstract Process Initialize();

        public abstract void MarkAsSelected();

        public abstract void MarkAsDeselected();

        public abstract void MarkAsOnline();

        public abstract void MarkAsOffline();


        public virtual XmlSchema GetSchema() {
            return null;
        }

        public virtual void ReadXml(XmlReader reader) {
            _xPos = reader.ReadElementContentAsInt();
            _yPos = reader.ReadElementContentAsInt();
        }

        public virtual void WriteXml(XmlWriter writer) {
            writer.WriteElementString(nameof(_xPos), _xPos.ToString());
            writer.WriteElementString(nameof(_yPos), _yPos.ToString());
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
    }
}