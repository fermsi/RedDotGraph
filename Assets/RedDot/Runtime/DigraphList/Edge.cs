namespace RedDot.Runtime.DigraphList
{
    public class Edge<Te>
    {
        public Te Data;
        public int ToVertexRank;
        public int Weight;
        public EType Etype = EType.Undetermined;

        public Edge(int toVertexRank, Te d = default(Te), int weight = 0)
        {
            this.ToVertexRank = toVertexRank;
            Data = d;
            this.Weight = weight;
        }
    }
}