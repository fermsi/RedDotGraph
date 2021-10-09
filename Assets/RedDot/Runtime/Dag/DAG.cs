using System;
using System.Collections.Generic;
using RedDot.Runtime.DigraphList;

namespace RedDot.Runtime.Dag
{
    /// <summary>
    /// 有向无环图
    /// </summary>
    /// <typeparam name="Tv"></typeparam>
    /// <typeparam name="Te"></typeparam>
    public class DAG<Tv, Te> : DigraphList.DiGraphList<Tv, Te>
    {
        /// <summary>
        /// 在(v, u)之间插入权重为weight的边e
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <param name="weight"></param>
        public override bool Insert(int v, int u, Te edge = default(Te), int weight = 0)
        {
            if (Exist(v, u))
            {
                Console.WriteLine($"插入边({v}, {u})失败，该边已经存在");
                return false;
            }
            
            Reset();//先重置状态
            SetStatus(v, VStatus.DISCOVERED);//标记开始点为被发现
            //检查是否有环
            if (checkCyclic(u))
            {
                Console.WriteLine($"插入边({v}, {u})失败，构成了环");
                return false;
            }

            var t = base.Insert(v, u, edge, weight);
            if(t)
                Console.WriteLine($"插入边({v}, {u})成功");
            return t;
        }

        /// <summary>
        /// 从r顶点开始检测是否有环，采用dfs方式
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private bool checkCyclic(int r)
        {
            List<Edge<Te>> temp;
            Stack<int> undiscoverStack = new Stack<int>();
            Stack<int> discoverStack = new Stack<int>();//用于通路回溯，记录当前通路的各个节点，回溯的时候，pop就行
            undiscoverStack.Push(r);
            while (undiscoverStack.Count > 0)
            {
                int index = undiscoverStack.Peek();
                if (discoverStack.Count == 0 || index != discoverStack.Peek())
                {
                    //这里可以在加上一个GetStatus(index) == VStatus.VISITED则undiscoverStack.Pop(); continue;的判断
                    
                    discoverStack.Push(index);
                    temp = outEdges[index];
                    SetStatus(index, VStatus.DISCOVERED);
                    for (int i = 0; i < temp.Count; i++)
                    {
                        var edge = temp[i];
                        if (GetStatus(edge.toVertexRank) == VStatus.DISCOVERED)
                        {
                            Console.Write($"{edge.toVertexRank} <- ");
                            while (discoverStack.Count > 0)
                            {
                                Console.Write($"{discoverStack.Pop()} <- ");
                            }
                            Console.Write($"{edge.toVertexRank}");
                            Console.WriteLine($"构成了环");
                            return true;
                        }
                        //else
                        if (GetStatus(edge.toVertexRank) == VStatus.UNDISCOVERED)
                            undiscoverStack.Push(edge.toVertexRank);
                    }
                }
                else
                {
                    SetStatus(undiscoverStack.Pop(), VStatus.VISITED);
                    discoverStack.Pop();
                }
            }
            
            return false;
        }
    }
}