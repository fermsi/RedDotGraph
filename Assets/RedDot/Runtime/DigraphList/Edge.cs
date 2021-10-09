namespace RedDot.Runtime.DigraphList
{
    public class Edge<Te>
    {
        public Te data;
        public int toVertexRank;
        public int weight;
        public EType etype = EType.UNDETERMINED;

        public Edge(int toVertexRank, Te d = default(Te), int weight = 0)
        {
            this.toVertexRank = toVertexRank;
            data = d;
            this.weight = weight;
        }
    }
}