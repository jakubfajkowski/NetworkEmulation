namespace NetworkUtilities {
    class Link {
        public static int linkCounter { get; private set; }
        public int linkNumber { get; }
        public int capacity { get; }

        public Link(int capacity) {
            linkCounter++;
            linkNumber = linkCounter;
            this.capacity = capacity;
        }
    }
}