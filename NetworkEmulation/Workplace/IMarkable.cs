namespace NetworkEmulation.Workplace {
    public interface IMarkable {
        void MarkAsSelected();
        void MarkAsDeselected();
        void MarkAsOnline();
        void MarkAsOffline();
    }
}