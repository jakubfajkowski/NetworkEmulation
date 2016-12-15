namespace NetworkEmulation.editor {
    public interface IMarkable {
        void MarkAsSelected();
        void MarkAsDeselected();
        void MarkAsOnline();
        void MarkAsOffline();
    }
}