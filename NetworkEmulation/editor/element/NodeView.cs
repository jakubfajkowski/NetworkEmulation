using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using NetworkUtilities;
using NetworkUtilities.Serialization;
using UniqueId = NetworkUtilities.UniqueId;

namespace NetworkEmulation.Editor.Element {
    public abstract class NodeView : ClippedPictureBox, IMarkable, IInitializable, ISerializable {
        public delegate void NodeMovingHandler(object sender);

        private Point _anchor;

        protected NodeView() {
            Id = UniqueId.Generate();
            CableCloudDataPort = PortRandomizer.RandomFreePort();
        }

        public int CableCloudDataPort { get; protected set; }

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



        public UniqueId Id { get; private set; }

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
                _anchor.X = e.X;
                _anchor.Y = e.Y;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Top += e.Y - _anchor.Y;
                Left += e.X - _anchor.X;

                NodeMoving();
            }
        }

        #region IXmlSerializable

        public virtual XmlSchema GetSchema() {
            return null;
        }

        public virtual void ReadXml(XmlReader reader) {
            reader.MoveToContent();
            var X = int.Parse(reader.GetAttribute("X"));
            var Y = int.Parse(reader.GetAttribute("Y"));
            Location = new Point(X, Y);
            Id = new UniqueId(reader.GetAttribute("Id"));
        }

        public virtual void WriteXml(XmlWriter writer) {
            writer.WriteAttributeString("X", Location.X.ToString());
            writer.WriteAttributeString("Y", Location.Y.ToString());
            writer.WriteAttributeString("Id", Id.ToString());
        }

        #endregion

    }
}