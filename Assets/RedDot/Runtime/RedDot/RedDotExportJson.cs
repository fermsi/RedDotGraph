using System;
using System.Collections.Generic;

namespace RedDot.Runtime.RedDot
{
    [Serializable]
    public class RedDotNodeJson
    {
        //红点节点全局key
        public string key;
        //红点状态类型
        public int redDotType;
        //逻辑ID
        public int externalId;
    }

    [Serializable]
    public class RedDotEdgeJson
    {
        //当前红点节点
        public string key;
        //红点子节点列表
        public List<string> outKeys = new List<string>();
    }
    
    [Serializable]
    public class RedDotExportJson
    {
        public List<RedDotNodeJson> vertexs = new List<RedDotNodeJson>();
        public List<RedDotEdgeJson> edges = new List<RedDotEdgeJson>();
    }
}