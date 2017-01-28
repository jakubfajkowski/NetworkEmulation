namespace NetworkUtilities.GraphAlgorithm.PriorityQueue
{
    public class Element<T> where T: new()
    {
        private double key;
        public double Key
        {
            get { return this.key; }
            set { this.key = value; }
        }
        private T data;
        public T Data
        {
            get { return this.data; }
        }

        public Element()
        {
            this.key = double.MaxValue;
            this.data = new T();
        }

        public Element(double key, T data)
        {
            this.key = key;
            this.data = data;
        }
    }
}
