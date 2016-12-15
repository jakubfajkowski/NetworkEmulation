namespace NetworkUtilities {
    internal class Link {
        public Link(int capacity) {
            LinkCounter++;
            LinkNumber = LinkCounter;
            Capacity = capacity;
        }

        public static int LinkCounter { get; private set; }
        public int LinkNumber { get; }
        public int Capacity { get; }
    }
}