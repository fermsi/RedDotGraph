using System;
using System.Collections.Generic;
using RedDot.Runtime.Dag;

namespace RedDot.Runtime.RedDot
{
    public class BaseRedDotGraph : DAG<RedDotVertex, int>
    {
        private static uint __time = 0;

        /// <summary>
        /// 获取当前时间——计数
        /// </summary>
        /// <returns></returns>
        protected static uint GetTime()
        {
            __time++;
            return __time;
        }
        
        protected Dictionary<string, int> vertexDict = new Dictionary<string, int>();
        
        /// <summary>
        /// 插入顶点
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public override int Insert(RedDotVertex v)
        {
            if (CheckHasRedDotByKey(v.key))
            {
                Console.WriteLine($"已经有此红点了, key: {v.key}");
                return -1;
            }
            int index = base.Insert(v);
            var vertex = GetVertex(index);
            // vertex.key = $"default_name_{index}";
            vertexDict[vertex.key] = index;
            return index;
        }

        /// <summary>
        /// 在(vex1, vex2)之间插入权重为weight的边e
        /// </summary>
        /// <param name="vex1"></param>
        /// <param name="vex2"></param>
        /// <param name="weight"></param>
        public bool InsertEdge(string vex1, string vex2, int weight = 0)
        {
            if (!vertexDict.TryGetValue(vex1, out int v))
            {
                Console.WriteLine($"没找到顶点：{vex1}");
                return false;
            }
            if (!vertexDict.TryGetValue(vex2, out int u))
            {
                Console.WriteLine($"没找到顶点：{vex2}");
                return false;
            }
            return base.Insert(v, u, 0, weight);
        }

        /// <summary>
        /// 判断是否有某个key的红点
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool CheckHasRedDotByKey(string key)
        {
            return vertexDict.ContainsKey(key);
        }

        /// <summary>
        /// 获取红点
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedDotVertex GetRedDotByKey(string key)
        {
            if (vertexDict.TryGetValue(key, out int index))
            {
                return GetVertex(index);
            }

            return null;
        }
    }
}