using System;

namespace NetworkUtilities.GraphAlgorithm.PriorityQueue {
    internal class Heap<T> : Queue<T> where T : new() {
        public void PushUp(int i) {
            while (i > 1 && Nodes[i - 1].Key > Nodes[i / 2 - 1].Key) {
                var temp = Nodes[i - 1];
                Nodes[i - 1] = Nodes[i / 2 - 1];
                Nodes[i / 2 - 1] = temp;
                i /= 2;
            }
        }

        public void PushDown(int i) {
            while (2 * i <= numberOfElements) {
                var j = i * 2;

                if (j > numberOfElements)
                    if (Nodes[i * 2 - 1].Key < Nodes[i * 2 + 1 - 1].Key)
                        j++;

                var temp = Nodes[i - 1];
                Nodes[i - 1] = Nodes[j - 1];
                Nodes[j - 1] = temp;
                i = j;
            }
        }

        public override void InsertElement(Element<T> e) {
            Nodes[++numberOfElements - 1] = e;
            PushUp(numberOfElements);
        }

        public override Element<T> DeleteMax() {
            //Check if the queue exists
            if (numberOfElements == 0)
                throw new SystemException("Kolejka nie istnieje.");

            var max = Nodes[0];

            Nodes[0] = Nodes[--numberOfElements];
            PushDown(1);

            Nodes[numberOfElements] = null;

            return max;
        }
    }
}