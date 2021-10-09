using System.Collections.Generic;

namespace RedDot.Runtime.DigraphList
{
    public partial class DiGraphList<Tv, Te>
    {
        //顶点

        /// <summary>
        /// 顶点列表
        /// </summary>
        protected List<Vertex<Tv>> vertices = new List<Vertex<Tv>>();
        /// <summary>
        /// 顶点总数
        /// </summary>
        protected int vNum = 0;

        /// <summary>
        /// 插入顶点
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public virtual int Insert(Tv v)
        {
            vertices.Add(new Vertex<Tv>(v));
            outEdges.Add(new List<Edge<Te>>());
            inEdges.Add(new List<int>());
            return vNum++;
        }

        /// <summary>
        /// 删除顶点及其关联边，返回该顶点信息
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual Tv Remove(int r)
        {
            Vertex<Tv> v = vertices[r];
            List<Edge<Te>> es = outEdges[r];
            //删除所有出边
            for (int i = es.Count - 1; i >= 0; i--)
            {
                Remove(r, es[i].toVertexRank);
            }

            //删除所有入边
            List<int> temp = inEdges[r];
            for (int i = temp.Count - 1; i >= 0; i--)
            {
                Remove(i, r);
            }
            
            //清空相关数据
            outEdges.RemoveAt(r);
            inEdges.RemoveAt(r);
            vertices.RemoveAt(r);
            vNum--;
            
            //以下效率性能不好，但是删除方法在游戏运行中一般是不用的，编辑器中可能需要
            //所有出度边大于r的节点减1
            for (int i = 0; i < outEdges.Count; i++)
            {
                foreach (var edge in outEdges[i])
                {
                    if (edge.toVertexRank > r) edge.toVertexRank--;
                }
            }
            //所有入度边大于r的节点减1
            foreach (var lInEdge in inEdges)
            {
                for (int i = 0; i < lInEdge.Count; i++)
                {
                    if (lInEdge[i] > r) lInEdge[i]--;
                }
            }
            
            return v.data;
        }

        /// <summary>
        /// 顶点v的数据（该顶点确实存在）
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual Tv GetVertex(int r)
        {
            return vertices[r].data;
        }

        /// <summary>
        /// 顶点v的入度（该顶点确实存在）
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual int InDegree(int r)
        {
            return inEdges[r].Count;
        }

        /// <summary>
        /// 顶点v的出度（该顶点确实存在）
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual int OutDegree(int r)
        {
            return outEdges[r].Count;
        }

        /// <summary>
        /// 顶点的首个邻接顶点，无邻接顶点的话返回-1
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual int FirstNbr(int r)
        {
            var edge = outEdges[r][0];
            if (edge != null) return edge.toVertexRank;
            return -1;
        }

        /// <summary>
        /// 顶点v的（相对于顶点j的）下一邻接顶点，这个算法暂未实现，性能有点低
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual int NextNbr(int r, int j)
        {
            return -1;
        }

        /// <summary>
        /// 顶点v的状态
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual VStatus GetStatus(int r)
        {
            return vertices[r].status;
        }

        /// <summary>
        /// 设置顶点v的状态
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual void SetStatus(int r, VStatus status)
        {
            vertices[r].status = status;
        }

        /// <summary>
        /// 获取顶点v事件标签dTime
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual int GetDTime(int r)
        {
            return vertices[r].dTime;
        }

        /// <summary>
        /// 设置顶点v事件标签dTime
        /// </summary>
        /// <param name="r"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public virtual void SetDTime(int r, int time)
        {
            vertices[r].dTime = time;
        }

        /// <summary>
        /// 获取顶点v事件标签fTime
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual int GetFTime(int r)
        {
            return vertices[r].fTime;
        }

        /// <summary>
        /// 设置顶点v事件标签fTime
        /// </summary>
        /// <param name="r"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public virtual void SetFTime(int r, int time)
        {
            vertices[r].fTime = time;
        }

        /// <summary>
        /// 获取顶点v在遍历树中的父亲
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual int GetParent(int r)
        {
            return vertices[r].parent;
        }

        /// <summary>
        /// 设置顶点v在遍历树中的父亲
        /// </summary>
        /// <param name="r"></param>
        /// <param name="pr">??????</param>
        /// <returns></returns>
        public virtual void SetParent(int r, int pr)
        {
            vertices[r].parent = pr;
        }

        /// <summary>
        /// 获取顶点v在遍历树中的优先级数
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual int GetPriority(int r)
        {
            return vertices[r].priority;
        }

        /// <summary>
        /// 设置顶点v在遍历树中的优先级数
        /// </summary>
        /// <param name="r"></param>
        /// <param name="priority">优先级</param>
        /// <returns></returns>
        public virtual void SetPriority(int r, int priority)
        {
            vertices[r].priority = priority;
        }
    }
}