namespace NetworkUtilities.GraphAlgorithm.PriorityQueue {
    public class Element<T> where T : new() {
        public Element() {
            Key = double.MaxValue;
            Data = new T();
        }

        public Element(double key, T data) {
            Key = key;
            Data = data;
        }

        public double Key { get; set; }

        public T Data { get; }
    }
}