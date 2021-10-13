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
            if (info.IsDataChange) return true;
            var edges = outEdges[index];
            //这里需要检查节点是否需要刷新
            //数红点的判断
            bool isNumType = CheckMaxDotTypeIs(v.RedDotType, RedDotStatus.RedDotStatusNumber);
            foreach (var edge in edges)
            {
                var uVertex = GetVertex(edge.ToVertexRank);
                var uInfo = uVertex.GetRuntimeInfo(id);
                if (uInfo.IsDataChange)
                {
                    //temp节点是数量节点，或者当前v节点状态不是数量状态，则需要重新刷数据
                    if(isNumType ? 
                        (CheckHasDotStatus(uVertex.RedDotType, RedDotStatus.RedDotStatusNumber)
                         || info.RedDotStatus != RedDotStatus.RedDotStatusNumber)
                        : (int)info.RedDotStatus <= uVertex.RedDotType)//非数量节点，只需要判断状态v.redDotStatus <= temp.redDotType即可
                    {
                        info.RedDotStatus = RedDotStatus.RedDotStatusNull;
                        info.IsDataChange = true;
                        _changedList.Add(v.Key);
                        return true;
                    }
                }
            }
            
            return false;
        }

        List<string> _changedList = new List<string>();
        /// <summary>
        /// 标记数据已经改变
        /// </summary>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <returns>数据有改变的列表</returns>
        public List<string> RedDotDataChange(string key, uint id = 0)
        {
            _changedList.Clear();
            if (VertexDict.TryGetValue(key, out int index))
            {
                var v = GetVertex(index);
                var info = v.GetRuntimeInfo(id);
                if(info.IsDataChange) return _changedList;
                info.RedDotStatus = RedDotStatus.RedDotStatusNull;
                info.IsDataChange = true;
                _changedList.Add(key);
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

                return _changedList;
            }
            Debug.LogWarning($"没有找到对应名字的红点节点：{key}");
            return _changedList;
        }

        /// <summary>
        /// 重新刷新红点的状态
        /// </summary>
        /// <param name="key"></param>
        /// <param name="id"></param>
        /// <param name="visitLeafVertex"></param>
        public void RefreshRedDotStatus(string key, uint id, Action<RedDotVertex, uint> visitLeafVertex)
        {
            if (VertexDict.TryGetValue(key, out int index))
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
        /// <param name="id"></param>
        /// <param name="visitLeafVertex"></param>
        private void RefreshRedDotStatus(int index, uint id, Action<RedDotVertex, uint> visitLeafVertex)
        {
            var v = GetVertex(index);
            var info = v.GetRuntimeInfo(id);
            if (!info.IsDataChange)
            {
                return;
            }

            if (v.ExternalId != -1)
            {
                visitLeafVertex(v, id);
//                info.isDataChange = false;
                info.StatusChangeTIme = GetTime();
//                return;//todo 这里应该有问题，之前设计的是，只有叶子节点才能有externalId，但是编辑器里是都有externalId，把这个口开开来得了
            }
            info.IsDataChange = false;

            bool isNumType = CheckMaxDotTypeIs(v.RedDotType, RedDotStatus.RedDotStatusNumber);
            var edges = outEdges[index];
            RedDotVertex uVertex;
            int nextIndex;
            for (int i = 0; i < edges.Count; i++)
            {
                nextIndex = edges[i].ToVertexRank;
                uVertex = GetVertex(nextIndex);
                var uInfo = uVertex.GetRuntimeInfo(id);
                //红点需要叠加，edges是按优先级排序的
                if (isNumType ? (int)info.RedDotStatus > uVertex.RedDotType : (int)info.RedDotStatus >= uVertex.RedDotType)
                {
                    return;//当前红点状态已经达到最新了，不需要在运算下去了
                }

                RefreshRedDotStatus(nextIndex, id, visitLeafVertex);
                //这段逻辑有待处理
                if (uInfo.RedDotStatus > info.RedDotStatus)
                {
                    info.RedDotStatus = uInfo.RedDotStatus;
                    info.Value = uInfo.Value;
                    info.StatusChangeTIme = GetTime();
                    if (!isNumType)
                    {
                        //不是数字红点，状态已经达到了即可停止
                        int status = (int) info.RedDotStatus;
                        if (status > v.RedDotType)
                        {
                            //算最高位
                            var num = v.RedDotType;
                            var highBit = -1;
                            while (num > 0)
                            {
                                highBit++;
                                num >>= 1;
                            }

                            info.RedDotStatus = (RedDotStatus)((int)Math.Pow(2, highBit));
                            return;
                        }
                        //因为是按位运算，所以如果redDotStatus是最大的话，那么redDotType不可能达到redDotStatus的两倍
                        if (v.RedDotType >> 1 < status)
                        {
                            return;
                        }
                    }
                }
                else if (info.RedDotStatus == RedDotStatus.RedDotStatusNumber && uInfo.RedDotStatus == RedDotStatus.RedDotStatusNumber)
                {
                    info.Value += uInfo.Value;
                    info.StatusChangeTIme = GetTime();
                }
            }
        }
    }
}