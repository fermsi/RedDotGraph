using System.IO;
using System.Text;
using UnityEngine;

namespace RedDot.Runtime.RedDot
{
    public partial class RedDotManager
    {
        public void AddJsonFile(string path, bool checkVisitor = true)
        {
            var jsonStr = File.ReadAllText(path, Encoding.UTF8);
            Debug.Log($"AddJsonFile path: {path}, jsonStr:{jsonStr}");
            AddJsonStr(jsonStr, checkVisitor);
        }
        
        public void AddJsonStr(string jsonStr, bool checkVisitor = true)
        {
            var cfg = JsonUtility.FromJson<RedDotExportJson>(jsonStr);
            AddExportJson(cfg, checkVisitor);
        }

        public void AddExportJson(RedDotExportJson config, bool checkVisitor = true)
        {
            foreach (var vertexJson in config.Vertexs)
            {
                var externalId = vertexJson.ExternalId;
                if (checkVisitor && externalId != -1)
                {
                    if (!RedDotExternalLogicVisitor.Ins().CheckHasExternalId(externalId))
                    {
                        Debug.LogError($"红点key:{vertexJson.Key}，对应的externalId:{externalId} func在RedDotExternalFuncVisitor中没有注册");
                    }
                }
                AddRedDotVertex(vertexJson.Key, vertexJson.RedDotType, vertexJson.ExternalId);
            }

            foreach (var edgeJson in config.Edges)
            {
                foreach (var outKey in edgeJson.OutKeys)
                {
                    AddRedDotEdge(edgeJson.Key, outKey);
                }
            }
        }
    }
}