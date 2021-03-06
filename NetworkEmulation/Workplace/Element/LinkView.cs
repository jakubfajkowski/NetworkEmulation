﻿using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using NetworkUtilities.Network;
using NetworkUtilities.Utilities.Serialization;
using UniqueId = NetworkUtilities.Utilities.UniqueId;

namespace NetworkEmulation.Workplace.Element {
    public partial class LinkView : Control, IMarkable, ISerializable {
        private static readonly Pen SelectedPen = new Pen(Color.Black, 5);
        private static readonly Pen DeselectedPen = new Pen(Color.Black, 1);
        private static readonly Pen OnlinePen = new Pen(Color.Green, 5);
        private static readonly Pen OfflinePen = new Pen(Color.Red, 5);
        private Pen _pen = DeselectedPen;

        public LinkView() {
            InitializeComponent();
            Id = UniqueId.Generate();
            Parameters = new LinkModel();
        }

        public LinkView(ref NodeView beginNodeView, ref NodeView endNodeView) : this() {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            SetAttachmentNodeViews(ref beginNodeView, ref endNodeView);
        }

        public NodeView BeginNodeView { get; private set; }
        public NodeView EndNodeView { get; private set; }

        public Point CenterPoint {
            get {
                var begin = BeginNodeView.CenterPoint();
                var end = EndNodeView.CenterPoint();

                int x = (begin.X + end.X)/2;
                int y = (begin.Y + end.Y)/2;

                return new Point(x, y);
            }
        }
        public LinkModel Parameters { get; set; }

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

        public UniqueId Id { get; private set; }

        public void SetAttachmentNodeViews(ref NodeView beginNodeView,
            ref NodeView endNodeView) {
            BeginNodeView = beginNodeView;
            EndNodeView = endNodeView;

            Parameters.BeginNodeViewId = BeginNodeView.Id;
            Parameters.EndNodeViewId = EndNodeView.Id;

            BeginNodeView.OnNodeMoving += sender => Parent.Refresh();
            EndNodeView.OnNodeMoving += sender => Parent.Refresh();
        }

        private void ChangeStyle(Pen pen) {
            _pen = pen;
            Parent.Refresh();
        }

        public void DrawLink(Graphics graphics) {
            var beginPoint = BeginNodeView.CenterPoint();
            var endPoint = EndNodeView.CenterPoint();

            graphics.DrawLine(_pen, beginPoint, endPoint);
        }

        public bool IsBetween(NodeView beginNodeView, NodeView endNodeView) {
            return BeginNodeView.Equals(beginNodeView) && EndNodeView.Equals(endNodeView) ||
                   BeginNodeView.Equals(endNodeView) && EndNodeView.Equals(beginNodeView);
        }

        #region IXmlSerializable

        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            reader.MoveToContent();
            Id = new UniqueId(reader.GetAttribute("Id"));
            reader.ReadStartElement(nameof(LinkView));
            Parameters = XmlSerializer.Deserialize<LinkModel>(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteAttributeString("Id", Id.ToString());
            XmlSerializer.Serialize(writer, Parameters);
        }

        #endregion
    }
}