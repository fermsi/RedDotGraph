using System;
using RedDot.Runtime.RedDot;

namespace RedDot.Editor.Data
{
    public class EditorRedDotManager
    {
        private static EditorRedDotManager s_ins = null;
        public static EditorRedDotManager Ins()
        {
            if (s_ins == null)
            {
                s_ins = new EditorRedDotManager();
            }
            return s_ins;
        }

        private EditorRedDotGraph _redDotGraph = new EditorRedDotGraph();

        private EditorRedDotManager()
        {
            
        }

        /// <summary>
        /// 添加红点顶点
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redDotType"></param>
        /// <param name="externalId"></param>
        /// <returns></returns>
        public int AddRedDotVertex(string key, int redDotType, int externalId = -1)
        {
            return _redDotGraph.Insert(new RedDotVertex(key, redDotType, externalId));
        }

        /// <summary>
        /// 添加边
        /// </summary>
        /// <param name="vex1"></param>
        /// <param name="vex2"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool AddRedDotEdge(string vex1, string vex2, int weight = 0)
        {
            return _redDotGraph.InsertEdge(vex1, vex2, weight);
        }
        
        /// <summary>
        /// 修改红点的key，key不能重名
        /// </summary>
        /// <param name="oldStr"></param>
        /// <param name="newStr"></param>
        /// <returns></returns>
        public bool ChangeRedDotKey(string oldStr, string newStr)
        {
            return _redDotGraph.ChangeRedDotKey(oldStr, newStr);
        }

        /// <summary>
        /// 把边的结束顶点oldEndVex，修改到newEndVex
        /// </summary>
        /// <param name="startVex"></param>
        /// <param name="oldEndVex"></param>
        /// <param name="newEndVex"></param>
        /// <returns></returns>
        public bool ChangeEdgeTo(string startVex, string oldEndVex, string newEndVex)
        {
            int r1 = _redDotGraph.GetVertexRank(startVex);
            int r2 = _redDotGraph.GetVertexRank(oldEndVex);
            if (r1 == -1 || r2 == -1) return false;
            var weight = _redDotGraph.GetWeight(r1, r2);
            //要修改的那条边并不会影响新添加边的回路
            //意思就是说：新添加的边，不会因为有边(startVex->oldEndVex)而影响其构成回路
            //所以先添加边即可
            if (AddRedDotEdge(startVex, newEndVex, weight))
            {
                _redDotGraph.Remove(r1, r2);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 修改边的权重
        /// </summary>
        /// <param name="vex1"></param>
        /// <param name="vex2"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool ChangeWeight(string vex1, string vex2, int weight)
        {
            int r1 = _redDotGraph.GetVertexRank(vex1);
            int r2 = _redDotGraph.GetVertexRank(vex2);
            if (r1 == -1 || r2 == -1)
            {
                Console.WriteLine("修改权重失败，节点没有获取到");
                return false;
            }
            _redDotGraph.SetWeight(r1, r2, weight);
            return true;
        }
    }
}