using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkEmulation.editor {
    public interface IMarkable {
        void MarkAsSelected();
        void MarkAsDeselected();
        void MarkAsOnline();
        void MarkAsOffline();
    }
}
