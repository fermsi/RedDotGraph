using System.Linq;
using RedDot.Runtime.RedDot;
using UnityEngine;

namespace RedDot.Editor.Data
{
    public class EditorRedDotGraph : BaseRedDotGraph
    {
        /// <summary>
        /// 修改红点类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="redDotType"></param>
        /// <returns></returns>
        public bool ChangeRedDotType(string key, int redDotType)
        {
            var v = GetRedDotByKey(key);
            if (v == null)
            {
//                Debug.Log($"没有找到对应名字的红点节点：{key}");
                return false;
            }

            v.redDotType = redDotType;
            
            return true;
        }

        public int RemoveEdge(string vex1, string vex2)
        {
            if (!vertexDict.TryGetValue(vex1, out int v))
            {
                Debug.Log($"没找到顶点：{vex1}");
                return -1;
            }
            if (!vertexDict.TryGetValue(vex2, out int u))
            {
                Debug.Log($"没找到顶点：{vex2}");
                return -1;
            }

            return Remove(v, u);
        }

        /// <summary>
        /// 修改红点的key，key不能重名
        /// </summary>
        /// <param name="oldStr"></param>
        /// <param name="newStr"></param>
        public bool ChangeRedDotKey(string oldStr, string newStr)
        {
            if (CheckHasRedDotByKey(newStr))
            {
                Debug.Log($"新名字已经被占用：{newStr}");
                return false;
            }
            var v = GetRedDotByKey(oldStr);
            if (v != null)
            {
                v.key = newStr;
                vertexDict[newStr] = vertexDict[oldStr];
                vertexDict.Remove(oldStr);
                return true;
            }

            Debug.Log($"没有找到对应名字的红点节点：{newStr}");
            return false;
        }
        
        /// <summary>
        /// 删除顶点及其关联边，返回该顶点信息
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public override RedDotVertex Remove(int r)
        {
            var vertex = GetVertex(r);
            var temp = vertexDict.Keys.ToList();
            //需要更新vertexDict中的其他key对应的index
            foreach (var key in temp)
            {
                if (vertexDict[key] > r)
                {
                    vertexDict[key] = vertexDict[key] - 1;
                }
            }
            vertexDict.Remove(vertex.key);
            return base.Remove(r);
        }
        
        /// <summary>
        /// 通过key删除顶点
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public RedDotVertex RemoveByKey(string key)
        {
            if (vertexDict.TryGetValue(key, out int index))
            {
                return Remove(index);
            }
            Debug.Log($"没有找到对应名字的红点节点：{key}");
            return null;
        }

        /// <summary>
        /// 获取顶点所在的秩，-1为返回失败
        /// </summary>
        /// <param name="vexKey"></param>
        /// <returns></returns>
        public int GetVertexRank(string vexKey)
        {
            if (!vertexDict.TryGetValue(vexKey, out int v))
            {
                Debug.Log($"没找到顶点：{vexKey}");
                return -1;
            }

            return v;
        }
    }
}