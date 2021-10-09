using System;
using System.Collections.Generic;
using RedDot.Runtime.DigraphList;
using UnityEngine;

namespace RedDot.Runtime.RedDot
{
    /// <summary>
    /// 红点图
    /// </summary>
    public class RedDotGraph : BaseRedDotGraph
    {
        /// <summary>
        /// 判断redDotType的最大红点类型是不是baseType的
        /// redDotType使用的是进制表示，具体的在RedDotConst.RED_DOT_TYPE_xxx
        /// </summary>
        /// <param name="redDotType"></param>
        /// <param name="baseStatus"></param>
        /// <returns></returns>
        private bool CheckMaxDotTypeIs(int redDotType, RedDotStatus baseStatus)
        {
            return redDotType >= (int)baseStatus && redDotType < (int)baseStatus << 1;
        }

        /// <summary>
        /// 检测是否含有baseType类红点
        /// </summary>
        /// <param name="redDotType"></param>
        /// <param name="baseStatus"></param>
        /// <returns></returns>
        private bool CheckHasDotStatus(int redDotType, RedDotStatus baseStatus)
        {
            return (redDotType & (int)baseStatus) > 0;
        }

        /// <summary>
        /// 判断节点是否要设置为isDataChange
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool RecheckDataChange(int index, uint id)
        {
            RedDotVertex v = GetVertex(index);
            var info = v.GetRuntimeInfo(id);
            if (info.isDataChange) return true;
            var edges = outEdges[index];
            Edge<int> edge;
            RedDotVertex uVertex;
            //这里需要检查节点是否需要刷新
            //数红点的判断
            bool isNumType = CheckMaxDotTypeIs(v.redDotType, RedDotStatus.RED_DOT_TYPE_NUMBER);
            for (int i = 0; i < edges.Count; i++)
            {
                edge = edges[i];
                uVertex = GetVertex(edge.toVertexRank);
                var uInfo = uVertex.GetRuntimeInfo(id);
                if (uInfo.isDataChange)
                {
                    //temp节点是数量节点，或者当前v节点状态不是数量状态，则需要重新刷数据
                    if(isNumType ? 
                        (CheckHasDotStatus(uVertex.redDotType, RedDotStatus.RED_DOT_TYPE_NUMBER)
                         || info.redDotStatus != RedDotStatus.RED_DOT_TYPE_NUMBER)
                       : (int)info.redDotStatus <= uVertex.redDotType)//非数量节点，只需要判断状态v.redDotStatus <= temp.redDotType即可
                    {
                        info.redDotStatus = RedDotStatus.RED_DOT_TYPE_NULL;
                        info.isDataChange = true;
                        changedList.Add(v.key);
                        return true;
                    }
                }
            }
            
            return false;
        }

        List<string> changedList = new List<string>();
        /// <summary>
        /// 标记数据已经改变
        /// </summary>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <returns>数据有改变的列表</returns>
        public List<string> RedDotDataChange(string key, uint id = 0)
        {
            changedList.Clear();
            RedDotVertex v;
            if (vertexDict.TryGetValue(key, out int index))
            {
                v = GetVertex(index);
                var info = v.GetRuntimeInfo(id);
                if(info.isDataChange) return changedList;
                info.redDotStatus = RedDotStatus.RED_DOT_TYPE_NULL;
                info.isDataChange = true;
                changedList.Add(key);
                //宽度优先
                Queue<int> needCheckQueue = new Queue<int>();
                needCheckQueue.Enqueue(index);
                // SetParent(index, -1);
                while (needCheckQueue.Count > 0)
                {
                    index = needCheckQueue.Dequeue();
                    if (RecheckDataChange(index, id))
                    {
                        var edges = inEdges[index];
                        foreach (var edge in edges)
                        {
                            //设置父节点，用于跟踪
                            // SetParent(edges[edge], index);//不能通过parent的方式来标记，有可能出现这种图：1->2->3,1->3
                            needCheckQueue.Enqueue(edges[edge]);
                        }
                    }
                }

                return changedList;
            }
            Debug.LogWarning($"没有找到对应名字的红点节点：{key}");
            return changedList;
        }

        /// <summary>
        /// 重新刷新红点的状态
        /// </summary>
        /// <param name="key"></param>
        public void RefreshRedDotStatus(string key, uint id, Action<RedDotVertex, uint> visitLeafVertex)
        {
            if (vertexDict.TryGetValue(key, out int index))
            {
                RefreshRedDotStatus(index, id, visitLeafVertex);
                return;
            }
            Console.WriteLine($"没有找到对应名字的红点节点：{key}, id：{id}");
        }

        /// <summary>
        /// 刷新具体的红点
        /// </summary>
        /// <param name="index"></param>
        private void RefreshRedDotStatus(int index, uint id, Action<RedDotVertex, uint> visitLeafVertex)
        {
            RedDotVertex v;
            v = GetVertex(index);
            var info = v.GetRuntimeInfo(id);
            if (!info.isDataChange)
            {
                return;
            }

            if (v.externalId != -1)
            {
                visitLeafVertex(v, id);
//                info.isDataChange = false;
                info.statusChangeTIme = GetTime();
//                return;//todo 这里应该有问题，之前设计的是，只有叶子节点才能有externalId，但是编辑器里是都有externalId，把这个口开开来得了
            }
            info.isDataChange = false;

            bool isNumType = CheckMaxDotTypeIs(v.redDotType, RedDotStatus.RED_DOT_TYPE_NUMBER);
            var edges = outEdges[index];
            RedDotVertex uVertex;
            int nextIndex;
            for (int i = 0; i < edges.Count; i++)
            {
                nextIndex = edges[i].toVertexRank;
                uVertex = GetVertex(nextIndex);
                var uInfo = uVertex.GetRuntimeInfo(id);
                //红点需要叠加，edges是按优先级排序的
                if (isNumType ? (int)info.redDotStatus > uVertex.redDotType : (int)info.redDotStatus >= uVertex.redDotType)
                {
                    return;//当前红点状态已经达到最新了，不需要在运算下去了
                }

                RefreshRedDotStatus(nextIndex, id, visitLeafVertex);
                //这段逻辑有待处理
                if (uInfo.redDotStatus > info.redDotStatus)
                {
                    info.redDotStatus = uInfo.redDotStatus;
                    info.value = uInfo.value;
                    info.statusChangeTIme = GetTime();
                    if (!isNumType)
                    {
                        //不是数字红点，状态已经达到了即可停止
                        int status = (int) info.redDotStatus;
                        if (status > v.redDotType)
                        {
                            //算最高位
                            var num = v.redDotType;
                            var highBit = -1;
                            while (num > 0)
                            {
                                highBit++;
                                num >>= 1;
                            }

                            info.redDotStatus = (RedDotStatus)((int)Math.Pow(2, highBit));
                            return;
                        }
                        //因为是按位运算，所以如果redDotStatus是最大的话，那么redDotType不可能达到redDotStatus的两倍
                        if (v.redDotType >> 1 < status)
                        {
                            return;
                        }
                    }
                }
                else if (info.redDotStatus == RedDotStatus.RED_DOT_TYPE_NUMBER && uInfo.redDotStatus == RedDotStatus.RED_DOT_TYPE_NUMBER)
                {
                    info.value += uInfo.value;
                    info.statusChangeTIme = GetTime();
                }
            }
        }
    }
}