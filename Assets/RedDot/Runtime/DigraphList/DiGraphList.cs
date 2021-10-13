namespace RedDot.Runtime.DigraphList
{
    //顶点状态
    public enum VStatus
    {
        Undiscovered,
        Discovered,
        Visited
    }

    //边在遍历树种所属的类型
    public enum EType
    {
        Undetermined,//未确定的
        Tree,
        Cross,
        Forward,
        Backward
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
                if (edge.ToVertexRank == u) return edge;
            }
            return null;
        }
        
        protected virtual void Reset()
        {
            for (int i = 0; i < VNum; i++)
            {
                SetStatus(i, VStatus.Undiscovered);
            }
        }
    }
}