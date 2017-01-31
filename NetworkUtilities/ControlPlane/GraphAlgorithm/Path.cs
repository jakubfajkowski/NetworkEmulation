namespace NetworkUtilities.ControlPlane.GraphAlgorithm {
    public class Path<T1> {
        public T1 Source { get; set; }
        public T1 Destination { get; set; }
        public Link Link { get; set; }
        public int Cost { get; set; } = 1;
    }
}