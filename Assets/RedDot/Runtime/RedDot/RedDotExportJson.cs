using System;
using System.Collections.Generic;

namespace RedDot.Runtime.RedDot
{
    [Serializable]
    public class RedDotNodeJson
    {
        //红点节点全局key
        public string Key;
        //红点状态类型
        public int RedDotType;
        //逻辑ID
        public int ExternalId;
    }

    [Serializable]
    public class RedDotEdgeJson
    {
        //当前红点节点
        public string Key;
        //红点子节点列表
        public List<string> OutKeys = new List<string>();
    }
    
    [Serializable]
    public class RedDotExportJson
    {
        public List<RedDotNodeJson> Vertexs = new List<RedDotNodeJson>();
        public List<RedDotEdgeJson> Edges = new List<RedDotEdgeJson>();
    }
}