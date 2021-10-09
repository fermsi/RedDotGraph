using System;

namespace RedDot.Runtime.DigraphList
{
    public class Vertex<Tv>
    {
        public Tv data;
        public VStatus status = VStatus.UNDISCOVERED;
        public int dTime = -1;
        public int fTime = -1;
        public int parent = -1;
        public int priority = Int32.MaxValue;

        public Vertex(Tv d = default(Tv))
        {
            data = d;
        }
    }
}