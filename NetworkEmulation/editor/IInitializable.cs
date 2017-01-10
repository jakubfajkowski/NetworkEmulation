using System.Diagnostics;

namespace NetworkEmulation.Editor {
    public interface IInitializable {
        Process Initialize();
    }
}