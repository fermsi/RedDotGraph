using System;

namespace RedDot.Runtime.DigraphList
{
    public class Vertex<Tv>
    {
        public Tv Data;
        public VStatus Status = VStatus.Undiscovered;
        public int DTime = -1;
        public int FTime = -1;
        public int Parent = -1;
        public int Priority = Int32.MaxValue;

        public Vertex(Tv d = default(Tv))
        {
            Data = d;
        }
    }
}