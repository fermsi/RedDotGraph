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
        private static int CurCount = 0;
        public List<RedDotNodeContext> NodeContexts = new List<RedDotNodeContext>();
        public RedDotEnumDict KeyEnumDict = new RedDotEnumDict();
        public RedDotEnumDict ExternalIdEnumDict = new RedDotEnumDict();
        private EditorRedDotGraph editorRedDotGraph = new EditorRedDotGraph();
        
        public RedDotNodeContext CreateNodeContext(Vector2 position)
        {
            var context = new RedDotNodeContext();
            context.Id = ++CurCount;
            var key = context.Id.ToString();
            context.Position = position;
            NodeContexts.Add(context);
            var vertex = new RedDotVertex(key, 0, -1);
            editorRedDotGraph.Insert(vertex);
            Dirty();
            return context;
        }

        public void ValidateGraph()
        {
            CurCount = 0;
            
            KeyEnumDict.ValidateData(this);
            ExternalIdEnumDict.ValidateData(this);
            
            foreach (var context in NodeContexts)
            {
                if (context.Id > CurCount)
                {
                    CurCount = context.Id;
                }
                var key = context.Id.ToString();
                var vertex = new RedDotVertex(key, 0, -1);
                editorRedDotGraph.Insert(vertex);
            }

            foreach (var context in NodeContexts)
            {
                foreach (var outId in context.OutNodeIds)
                {
                    if (!editorRedDotGraph.InsertEdge(context.Id.ToString(), outId.ToString()))
                    {
                        Debug.Log($"out:{context.Id}, in:{outId} insert edge fail");
                    }
                    else
                    {
                        Debug.Log($"out:{context.Id}, in:{outId} insert edge success");
                        
                    }
                }
            }
        }

        public bool RemoveNodeContext(RedDotNodeContext nodeContext)
        {
//            Debug.Log($"==========RemoveNodeContext:{nodeContext}");
            if (nodeContext != null)
            {
                if (editorRedDotGraph.RemoveByKey(nodeContext.Id.ToString()) != null)
                {
//                    Debug.Log($"==========RemoveNodeContext:RemoveByKey");
                    Dirty();
                    return NodeContexts.Remove(nodeContext);
                }
//                Debug.Log($"==========RemoveNodeContext:fail");
            }
            return false;
        }

        public bool RemoveEdge(RedDotNodeContext inNodeContext, RedDotNodeContext outNodeContext)
        {
            if (editorRedDotGraph.RemoveEdge(outNodeContext.Id.ToString(), inNodeContext.Id.ToString()) != -1)
            {
                outNodeContext.RemoveOutNode(inNodeContext.Id);
                Dirty();
                return true;
            }

            return false;
        }

        public bool AddEdge(RedDotNodeContext inNodeContext, RedDotNodeContext outNodeContext)
        {
            var success = editorRedDotGraph.InsertEdge(outNodeContext.Id.ToString(), inNodeContext.Id.ToString());
            if (success)
            {
                outNodeContext.AddOutNode(inNodeContext.Id);
                Dirty();
            }

            return success;
        }

        private bool dirty = false;
        public void Dirty()
        {
            dirty = true;
        }

        public bool IsDirty => dirty;

        public bool SaveTo(string pathName)
        {
            CacheData();
            FileUtilities.WriteRedDotGraphDataToDisk(pathName, this);
            AssetDatabase.Refresh();
            dirty = false;
            return true;
        }

        public void CacheData()
        {
            KeyEnumDict.PrepareExport();
            ExternalIdEnumDict.PrepareExport();
        }

        public bool IsDataCached()
        {
            return KeyEnumDict.IsCached && ExternalIdEnumDict.IsCached;
        }

        public void ExportTo(string pathName)
        {
            var exportJson = new RedDotExportJson();
            var nodeContextDict = new Dictionary<int, RedDotNodeContext>();
            foreach (var nodeContext in NodeContexts)
            {
                nodeContextDict[nodeContext.Id] = nodeContext;
                var node = new RedDotNodeJson();
                node.Key = KeyEnumDict.GetKey(nodeContext.KeyId);
                node.RedDotType = nodeContext.RedDotType;
                node.ExternalId = nodeContext.FuncId;
                exportJson.Vertexs.Add(node);
            }

            foreach (var nodeContext in NodeContexts)
            {
                var edge = new RedDotEdgeJson();
                edge.Key = KeyEnumDict.GetKey(nodeContext.KeyId);
                foreach (var id in nodeContext.OutNodeIds)
                {
                    edge.OutKeys.Add(KeyEnumDict.GetKey(nodeContextDict[id].KeyId));
                }
                exportJson.Edges.Add(edge);
            }
            FileUtilities.WriteRedDotGraphDataToDisk(pathName, exportJson);
            AssetDatabase.Refresh();
        }
    }
}