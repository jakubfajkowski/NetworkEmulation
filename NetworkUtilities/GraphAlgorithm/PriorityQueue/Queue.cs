namespace NetworkUtilities.GraphAlgorithm.PriorityQueue
{
    abstract class Queue<T> where T : new()
    {
        protected Element<T>[] nodes;
        protected int numberOfElements;
        public int NumberOfElements
        {
            get { return numberOfElements; }
        }

        public void initialise(int length)
        {
            nodes = new Element<T>[length];
        }
        abstract public void insertElement(Element<T> e);
        abstract public Element<T> deleteMax();
    }
}
