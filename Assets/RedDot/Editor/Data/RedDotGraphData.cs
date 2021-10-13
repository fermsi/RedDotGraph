using System;
using System.Collections.Generic;
using RedDot.Editor.Util;
using RedDot.Runtime.RedDot;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RedDot.Editor.Data
{
    [Serializable]
    public class RedDotGraphData
    {
        private static int s_curCount = 0;
        public List<RedDotNodeContext> nodeContexts = new List<RedDotNodeContext>();
        public RedDotEnumDict keyEnumDict = new RedDotEnumDict();
        public RedDotEnumDict externalIdEnumDict = new RedDotEnumDict();
        private EditorRedDotGraph _editorRedDotGraph = new EditorRedDotGraph();
        
        public RedDotNodeContext CreateNodeContext(Vector2 position)
        {
            var context = new RedDotNodeContext();
            context.id = ++s_curCount;
            var key = context.id.ToString();
            context.position = position;
            nodeContexts.Add(context);
            var vertex = new RedDotVertex(key, 0, -1);
            _editorRedDotGraph.Insert(vertex);
            Dirty();
            return context;
        }

        public void ValidateGraph()
        {
            s_curCount = 0;
            
            keyEnumDict.ValidateData(this);
            externalIdEnumDict.ValidateData(this);
            
            foreach (var context in nodeContexts)
            {
                if (context.id > s_curCount)
                {
                    s_curCount = context.id;
                }
                var key = context.id.ToString();
                var vertex = new RedDotVertex(key, 0, -1);
                _editorRedDotGraph.Insert(vertex);
            }

            foreach (var context in nodeContexts)
            {
                foreach (var outId in context.outNodeIds)
                {
                    if (!_editorRedDotGraph.InsertEdge(context.id.ToString(), outId.ToString()))
                    {
                        Debug.Log($"out:{context.id}, in:{outId} insert edge fail");
                    }
                    else
                    {
                        Debug.Log($"out:{context.id}, in:{outId} insert edge success");
                        
                    }
                }
            }
        }

        public bool RemoveNodeContext(RedDotNodeContext nodeContext)
        {
//            Debug.Log($"==========RemoveNodeContext:{nodeContext}");
            if (nodeContext != null)
            {
                if (_editorRedDotGraph.RemoveByKey(nodeContext.id.ToString()) != null)
                {
//                    Debug.Log($"==========RemoveNodeContext:RemoveByKey");
                    Dirty();
                    return nodeContexts.Remove(nodeContext);
                }
//                Debug.Log($"==========RemoveNodeContext:fail");
            }
            return false;
        }

        public bool RemoveEdge(RedDotNodeContext inNodeContext, RedDotNodeContext outNodeContext)
        {
            if (_editorRedDotGraph.RemoveEdge(outNodeContext.id.ToString(), inNodeContext.id.ToString()) != -1)
            {
                outNodeContext.RemoveOutNode(inNodeContext.id);
                Dirty();
                return true;
            }

            return false;
        }

        public bool AddEdge(RedDotNodeContext inNodeContext, RedDotNodeContext outNodeContext)
        {
            var success = _editorRedDotGraph.InsertEdge(outNodeContext.id.ToString(), inNodeContext.id.ToString());
            if (success)
            {
                outNodeContext.AddOutNode(inNodeContext.id);
                Dirty();
            }

            return success;
        }

        private bool _dirty = false;
        public void Dirty()
        {
            _dirty = true;
        }

        public bool IsDirty => _dirty;

        public bool SaveTo(string pathName)
        {
            CacheData();
            FileUtilities.WriteRedDotGraphDataToDisk(pathName, this);
            AssetDatabase.Refresh();
            _dirty = false;
            return true;
        }

        public void CacheData()
        {
            keyEnumDict.PrepareExport();
            externalIdEnumDict.PrepareExport();
        }

        public bool IsDataCached()
        {
            return keyEnumDict.IsCached && externalIdEnumDict.IsCached;
        }

        public void ExportTo(string pathName)
        {
            var exportJson = new RedDotExportJson();
            var nodeContextDict = new Dictionary<int, RedDotNodeContext>();
            foreach (var nodeContext in nodeContexts)
            {
                nodeContextDict[nodeContext.id] = nodeContext;
                var node = new RedDotNodeJson();
                node.key = keyEnumDict.GetKey(nodeContext.keyId);
                node.redDotType = nodeContext.redDotType;
                node.externalId = nodeContext.funcId;
                exportJson.vertexs.Add(node);
            }

            foreach (var nodeContext in nodeContexts)
            {
                var edge = new RedDotEdgeJson();
                edge.key = keyEnumDict.GetKey(nodeContext.keyId);
                foreach (var id in nodeContext.outNodeIds)
                {
                    edge.outKeys.Add(keyEnumDict.GetKey(nodeContextDict[id].keyId));
                }
                exportJson.edges.Add(edge);
            }
            FileUtilities.WriteRedDotGraphDataToDisk(pathName, exportJson);
            AssetDatabase.Refresh();
        }
    }
}