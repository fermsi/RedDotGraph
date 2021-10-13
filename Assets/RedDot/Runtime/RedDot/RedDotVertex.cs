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
        public uint StatusChangeTIme = 0;
        /// <summary>
        /// 红点当前状态，运行时才知道的
        /// </summary>
        public RedDotStatus RedDotStatus = 0;
        /// <summary>
        /// 红点值
        /// </summary>
        public int Value = 0;
        /// <summary>
        /// 数据是否有更新
        /// </summary>
        public bool IsDataChange;
    }
    /// <summary>
    /// 红点顶点
    /// </summary>
    public class RedDotVertex
    {
        public string Key;
        /// <summary>
        /// 红点类型，红点需要显示的状态
        /// </summary>
        public int RedDotType = 0;
        /// <summary>
        /// 外部接口ID
        /// </summary>
        public int ExternalId = -1;

        private Dictionary<uint, RedDotVertexRuntimeInfo> _runTimeInfos = new Dictionary<uint, RedDotVertexRuntimeInfo>();

        /// <summary>
        /// 获取运行时信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RedDotVertexRuntimeInfo GetRuntimeInfo(uint id)
        {
            if (_runTimeInfos.TryGetValue(id, out var info))
            {
                return info;
            }
            info = new RedDotVertexRuntimeInfo();
            _runTimeInfos[id] = info;
            return info;
        }

        public RedDotVertex(string key, int redDotType, int externalId)
        {
            Key = key;
            RedDotType = redDotType;
            ExternalId = externalId;
        }
    }
}