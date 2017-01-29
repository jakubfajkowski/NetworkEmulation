namespace NetworkUtilities.GraphAlgorithm.PriorityQueue {
    internal abstract class Queue<T> where T : new() {
        protected Element<T>[] Nodes;
        protected int numberOfElements;

        public int NumberOfElements {
            get { return numberOfElements; }
        }

        public void Initialise(int length) {
            Nodes = new Element<T>[length];
        }

        public abstract void InsertElement(Element<T> e);
        public abstract Element<T> DeleteMax();
    }
}