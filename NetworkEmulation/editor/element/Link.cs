using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NetworkEmulation.network.element;

namespace NetworkEmulation.editor.element {
    public partial class Link : Control, IMarkable, IInitializable, IXmlSerializable {
        private static readonly Pen SelectedPen = new Pen(Color.Black, 5);
        private static readonly Pen DeselectedPen = new Pen(Color.Black, 1);
        private static readonly Pen OnlinePen = new Pen(Color.Green, 1);
        private static readonly Pen OfflinePen = new Pen(Color.Red, 1);
        private readonly NodePictureBox _beginNodePictureBox;
        private readonly NodePictureBox _endNodePictureBox;
        private Pen _pen = DeselectedPen;

        public Link(ref NodePictureBox beginNodePictureBox, ref NodePictureBox endNodePictureBox) {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            _beginNodePictureBox = beginNodePictureBox;
            _endNodePictureBox = endNodePictureBox;


            _beginNodePictureBox.OnNodeMoving += sender => Parent.Refresh();
            _endNodePictureBox.OnNodeMoving += sender => Parent.Refresh();
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
            var parametersSerializer = new XmlSerializer(typeof(LinkSerializableParameters));

            reader.ReadStartElement(nameof(Link));
            Parameters = parametersSerializer.Deserialize(reader) as LinkSerializableParameters;
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            var parametersSerializer = new XmlSerializer(typeof(LinkSerializableParameters));

            parametersSerializer.Serialize(writer, Parameters);
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