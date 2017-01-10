namespace NetworkEmulation.Editor {
    public interface IMarkable {
        void MarkAsSelected();
        void MarkAsDeselected();
        void MarkAsOnline();
        void MarkAsOffline();
    }
}