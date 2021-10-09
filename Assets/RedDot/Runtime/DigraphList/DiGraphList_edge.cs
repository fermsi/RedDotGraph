using System;
using System.Collections.Generic;

namespace RedDot.Runtime.DigraphList
{
    public partial class DiGraphList<Tv, Te>
    {
        /// <summary>
        /// 出度边
        /// </summary>
        protected List<List<Edge<Te>>> outEdges = new List<List<Edge<Te>>>();
        /// <summary>
        /// 入度边
        /// </summary>
        protected List<List<int>> inEdges = new List<List<int>>();
        /// <summary>
        /// 边的总数
        /// </summary>
        protected int eNum = 0;

        /// <summary>
        /// 边(v, u)是否存在
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public virtual bool Exist(int v, int u)
        {
            foreach (var edge in outEdges[v])
            {
                if (edge.toVertexRank == u) return true;
            }
            return false;
        }

        /// <summary>
        /// 在(v, u)之间插入权重为weight的边e
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <param name="weight"></param>
        public virtual bool Insert(int v, int u, Te edge = default(Te), int weight = 0)
        {
            if (Exist(v, u))
            {
                Console.WriteLine($"插入边({v}, {u})失败，该边已经存在");
                return false;
            }
            outEdges[v].Add(new Edge<Te>(u, edge, weight));
            inEdges[u].Add(v);
            eNum++;
            return true;
        }

        /// <summary>
        /// 删除顶点(v, u)之间的边e，并返回该边信息，目前只有编辑器里会用到，不做性能优化了
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public virtual Te Remove(int v, int u)
        {
            for (int edgeIndex = 0; edgeIndex < outEdges[v].Count; edgeIndex++)
            {
                if (outEdges[v][edgeIndex].toVertexRank == u)
                {
                    for (int inEdgeIndex = 0; inEdgeIndex < inEdges[u].Count; inEdgeIndex++)
                    {
                        if (inEdges[u][inEdgeIndex] == v)
                        {
                            inEdges[u].RemoveAt(inEdgeIndex);//删除入度
                            break;
                        }
                    }

                    eNum--;
                    var temp = outEdges[v][edgeIndex];
                    outEdges[v].RemoveAt(edgeIndex);//删除出度
                    return temp.data;
                }
            }
            return default(Te);
        }

        /// <summary>
        /// 获取边(v, u)的类型
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public virtual EType GetEType(int v, int u)
        {
            var edge = GetEdgeObj(v, u);
            if (edge != null) return edge.etype;
            return EType.UNDETERMINED;
        }

        /// <summary>
        /// 设置边(v, u)的类型
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <param name="type"></param>
        public virtual void SetEType(int v, int u, EType type)
        {
            var edge = GetEdgeObj(v, u);
            if (edge != null) edge.etype = type;
        }

        /// <summary>
        /// 获取边(v, u)的数据
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public virtual Te GetEdge(int v, int u)
        {
            var edge = GetEdgeObj(v, u);
            if (edge != null) return edge.data;
            return default(Te);
        }

        /// <summary>
        /// 设置边(v, u)的数据
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public virtual void SetEdge(int v, int u, Te edge)
        {
            var edgeObj = GetEdgeObj(v, u);
            if (edgeObj != null) edgeObj.data = edge;
        }

        /// <summary>
        /// 获取边(v, u)的权重
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public virtual int GetWeight(int v, int u)
        {
            var edge = GetEdgeObj(v, u);
            if (edge != null) return edge.weight;
            return 0;
        }

        /// <summary>
        /// 设置边(v, u)的权重
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <param name="weight"></param>
        public virtual void SetWeight(int v, int u, int weight)
        {
            var edge = GetEdgeObj(v, u);
            if (edge != null) edge.weight = weight;
        }

        /// <summary>
        ///  把边(v, oldU)改为(v, newU)，目前只有编辑器有用，效率没有做优化
        /// </summary>
        /// <param name="v"></param>
        /// <param name="oldU"></param>
        /// <param name="newU"></param>
        public virtual void ChangeEdgeTo(int v, int oldU, int newU)
        {
            var oldEdge = GetEdgeObj(v, oldU);
            Insert(v, newU, oldEdge.data, oldEdge.weight);//先添加
            Remove(v, oldU);//后删除
        }
    }
}