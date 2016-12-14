using System.Collections.Generic;
using System.Drawing;

namespace NetworkEmulation.editor {
    public class Connection : IMarkable{
        private List<Link> _links;

        public Connection(List<Link> links) {
            _links = links;

            MarkAsSelected();
        }

        public void MarkAsSelected() {
            _links.ForEach(link => link.MarkAsSelected());
        }

        public void MarkAsDeselected() {
            _links.ForEach(link => link.MarkAsDeselected());
        }

        public void MarkAsOnline() {
            _links.ForEach(link => link.MarkAsOnline());
        }

        public void MarkAsOffline() {
            _links.ForEach(link => link.MarkAsOffline());
        }
    }
}
