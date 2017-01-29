namespace NetworkUtilities.GraphAlgorithm.PriorityQueue {
    internal abstract class Queue<T> where T : new() {
        protected Element<T>[] nodes;
        protected int numberOfElements;

        public int NumberOfElements {
            get { return numberOfElements; }
        }

        public void initialise(int length) {
            nodes = new Element<T>[length];
        }

        public abstract void insertElement(Element<T> e);
        public abstract Element<T> deleteMax();
    }
}