using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using NetworkEmulation.network.element;
using NetworkUtilities;
using UniqueId = NetworkUtilities.UniqueId;
using XmlSerializer = NetworkUtilities.XmlSerializer;

namespace NetworkEmulation.editor.element {
    public partial class Link : Control, IMarkable, IInitializable, ISerializable {
        private static readonly Pen SelectedPen = new Pen(Color.Black, 5);
        private static readonly Pen DeselectedPen = new Pen(Color.Black, 1);
        private static readonly Pen OnlinePen = new Pen(Color.Green, 1);
        private static readonly Pen OfflinePen = new Pen(Color.Red, 1);
        private NodePictureBox _beginNodePictureBox;
        private NodePictureBox _endNodePictureBox;
        private Pen _pen = DeselectedPen;

        public Link() {
            Id = UniqueId.Generate();
        }

        public Link(ref NodePictureBox beginNodePictureBox, ref NodePictureBox endNodePictureBox) {
            Id = UniqueId.Generate();
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            SetAttachmentNodePictureBoxes(ref beginNodePictureBox, ref endNodePictureBox);
        }

        public LinkSerializableParameters Parameters { get; set; }

        public Process Initialize() {
            throw new NotImplementedException();
        }

        public void MarkAsSelected() {
            ChangeStyle(SelectedPen);
        }

        public void MarkAsDeselected() {
            ChangeStyle(DeselectedPen);
        }

        public void MarkAsOnline() {
            ChangeStyle(OnlinePen);
        }

        public void MarkAsOffline() {
            ChangeStyle(OfflinePen);
        }

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            reader.MoveToContent();
            Id = new UniqueId(reader.GetAttribute("Id"));
            reader.ReadStartElement(nameof(Link));
            Parameters = XmlSerializer.Deserialize<LinkSerializableParameters>(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteAttributeString("Id", Id.ToString());
            XmlSerializer.Serialize(writer, Parameters);
        }

        public UniqueId Id { get; private set; }

        public void SetAttachmentNodePictureBoxes(ref NodePictureBox beginNodePictureBox,
            ref NodePictureBox endNodePictureBox) {
            _beginNodePictureBox = beginNodePictureBox;
            _endNodePictureBox = endNodePictureBox;

            Parameters.BeginNodePictureBoxId = _beginNodePictureBox.Id;
            Parameters.EndNodePictureBoxId = _endNodePictureBox.Id;

            _beginNodePictureBox.OnNodeMoving += sender => Parent.Refresh();
            _endNodePictureBox.OnNodeMoving += sender => Parent.Refresh();
        }

        private void ChangeStyle(Pen pen) {
            _pen = pen;
            Parent.Refresh();
        }

        public void DrawLink(Graphics graphics) {
            var beginPoint = _beginNodePictureBox.CenterPoint();
            var endPoint = _endNodePictureBox.CenterPoint();

            graphics.DrawLine(_pen, beginPoint, endPoint);
        }

        public bool IsBetween(NodePictureBox beginNodePictureBox, NodePictureBox endNodePictureBox) {
            return (_beginNodePictureBox.Equals(beginNodePictureBox) && _endNodePictureBox.Equals(endNodePictureBox)) ||
                   (_beginNodePictureBox.Equals(endNodePictureBox) && _endNodePictureBox.Equals(beginNodePictureBox));
        }
    }
}