namespace RedDot.Runtime.DigraphList
{
    //顶点状态
    public enum VStatus
    {
        UNDISCOVERED,
        DISCOVERED,
        VISITED
    }

    //边在遍历树种所属的类型
    public enum EType
    {
        UNDETERMINED,//未确定的
        TREE,
        CROSS,
        FORWARD,
        BACKWARD
    }
    
    /// <summary>
    /// 有向图，邻接链表
    /// </summary>
    /// <typeparam name="Tv"></typeparam>
    /// <typeparam name="Te"></typeparam>
    public partial class DiGraphList<Tv, Te>
    {
        private Edge<Te> GetEdgeObj(int v, int u)
        {
            foreach (var edge in outEdges[v])
            {
                if (edge.toVertexRank == u) return edge;
            }
            return null;
        }
        
        protected virtual void Reset()
        {
            for (int i = 0; i < vNum; i++)
            {
                SetStatus(i, VStatus.UNDISCOVERED);
            }
        }
    }
}