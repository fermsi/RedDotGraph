using System.Collections.Generic;

namespace RedDot.Runtime.RedDot
{
    /// <summary>
    /// 红点节点运行时信息
    /// </summary>
    public class RedDotVertexRuntimeInfo
    {
        /// <summary>
        /// 状态改变时间
        /// </summary>
        public uint statusChangeTIme = 0;
        /// <summary>
        /// 红点当前状态，运行时才知道的
        /// </summary>
        public RedDotStatus redDotStatus = 0;
        /// <summary>
        /// 红点值
        /// </summary>
        public int value = 0;
        /// <summary>
        /// 数据是否有更新
        /// </summary>
        public bool isDataChange;
    }
    /// <summary>
    /// 红点顶点
    /// </summary>
    public class RedDotVertex
    {
        public string key;
        /// <summary>
        /// 红点类型，红点需要显示的状态
        /// </summary>
        public int redDotType = 0;
        /// <summary>
        /// 外部接口ID
        /// </summary>
        public int externalId = -1;

        private Dictionary<uint, RedDotVertexRuntimeInfo> runTimeInfos = new Dictionary<uint, RedDotVertexRuntimeInfo>();

        /// <summary>
        /// 获取运行时信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RedDotVertexRuntimeInfo GetRuntimeInfo(uint id)
        {
            RedDotVertexRuntimeInfo info;
            if (runTimeInfos.TryGetValue(id, out info))
            {
                return info;
            }
            info = new RedDotVertexRuntimeInfo();
            runTimeInfos[id] = info;
            return info;
        }

        public RedDotVertex(string key, int redDotType, int externalId)
        {
            this.key = key;
            this.redDotType = redDotType;
            this.externalId = externalId;
        }
    }
}