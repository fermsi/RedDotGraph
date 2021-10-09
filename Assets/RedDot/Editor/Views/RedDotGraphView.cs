using System;
using System.Collections.Generic;
using RedDot.Editor.Config;
using RedDot.Editor.Data;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedDot.Editor.Views
{
    public class RedDotGraphView : GraphView
    {
        private RedDotWindow editorWindow;
        private RedDotGraphData graphData;
        public RedDotGraphView(RedDotWindow editorWindow, RedDotGraphData graphData)
        {
            this.graphData = graphData;
            this.editorWindow = editorWindow;

            //按照父级的宽高全屏填充
            this.StretchToParentSize();
            //滚轮缩放
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            //graphview窗口内容的拖动
            this.AddManipulator(new ContentDragger());
            //选中Node移动功能
            this.AddManipulator(new SelectionDragger());
            //多个node框选功能
            this.AddManipulator(new RectangleSelector());
        

            var menuWindowProvider = ScriptableObject.CreateInstance<RedDotSearchWindowProvider>();
            menuWindowProvider.OnSelectEntryHandler = OnMenuSelectEntry;
        
            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(
                    context.screenMousePosition
                    ), 
                    menuWindowProvider);
            };

            graphViewChanged += RedDotGraphViewChanged;

            var toolbar = new IMGUIContainer(() =>
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);
                    if (GUILayout.Button("保存", EditorStyles.toolbarButton))
                    {
//                        graphData.SaveTo(editorWindow.GetCurAssetPath());
                        editorWindow.CheckAndSave();
                    }
                    GUILayout.Space(6);
                    if (GUILayout.Button("导出红点配置", EditorStyles.toolbarButton))
                    {
                        editorWindow.ExportTo();
                    }
                    GUILayout.Space(6);
                    if (GUILayout.Button("Key设置", EditorStyles.toolbarButton))
                    {
                        KeySetting();
                    }
                    GUILayout.Space(6);
                    if (GUILayout.Button("逻辑ID设置", EditorStyles.toolbarButton))
                    {
                        ExternalIdSetting();
                    }
                    GUILayout.EndHorizontal();
                });
            Add(toolbar);

            InitElements();
        }

        private void KeySetting()
        {
            Debug.Log("Key设置");
            if (editorWindow.keySettingWindow != null)
            {
                editorWindow.keySettingWindow.Focus();
                return;
            }
            var window = EditorWindow.CreateWindow<RedDotEnumDictWindow>(typeof(RedDotEnumDictWindow), typeof(RedDotWindow));
            window.Initialize(graphData.KeyEnumDict, RedDotEditorConfig.KEY_WINDOW_TITLE);
            window.Focus();
            editorWindow.keySettingWindow = window;
        }

        private void ExternalIdSetting()
        {
            Debug.Log("逻辑ID设置 ");
            if (editorWindow.externalIdSettingWindow != null)
            {
                editorWindow.externalIdSettingWindow.Focus();
                return;
            }
            var window = EditorWindow.CreateWindow<RedDotEnumDictWindow>(typeof(RedDotEnumDictWindow), typeof(RedDotWindow));
            window.Initialize(graphData.ExternalIdEnumDict, RedDotEditorConfig.EXTERNAL_ID_WINDOW_TITLE);
            window.Focus();
            editorWindow.externalIdSettingWindow = window;
        }

        private void InitElements()
        {
            Dictionary<int, RedDotNodeView> views = new Dictionary<int, RedDotNodeView>();
            foreach (var context in graphData.NodeContexts)
            {
                var node = new RedDotNodeView(context, graphData);
                AddElement(node);
                views[context.Id] = node;
            }

            foreach (var context in graphData.NodeContexts)
            {
                var outputNode = views[context.Id];
                foreach (var outNodeId in context.OutNodeIds)
                {
                    var inputNode = views[outNodeId];
                    outputNode.outPort.ConnectTo(inputNode.inPort);
                    var edge = new Edge()
                    {
                        output = outputNode.outPort,
                        input = inputNode.inPort,
                        pickingMode = PickingMode.Position,
                    };
                    AddElement(edge);
                }
                
            }
        }
    
        private bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var type = searchTreeEntry.userData as Type;
            var windowRoot = editorWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - editorWindow.position.position);
            var graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);
            
            Node node = Activator.CreateInstance(type, graphData.CreateNodeContext(graphMousePosition), graphData) as Node;
            AddElement(node);
            return true;
        }
    
        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            foreach (var port in ports.ToList())
            {
                if (startAnchor.node == port.node ||
                    startAnchor.direction == port.direction ||
                    startAnchor.portType != port.portType)
                {
                    continue;
                }

                compatiblePorts.Add(port);
            }
            return compatiblePorts;
        }

        private GraphViewChange RedDotGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                foreach (var node in graphViewChange.movedElements)
                {
                    (node as RedDotNodeView).NodeChanged();
                    graphData.Dirty();
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                List<Edge> addFailEdges = new List<Edge>();
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    var inNode = edge.input.node as RedDotNodeView;
                    var outNode = edge.output.node as RedDotNodeView;
                    if (!graphData.AddEdge(inNode.GetContext(), outNode.GetContext()))
                    {
                        addFailEdges.Add(edge);
                    }
                }

                foreach (var edge in addFailEdges)
                {
                    graphViewChange.edgesToCreate.Remove(edge);
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var element in graphViewChange.elementsToRemove)
                {
                    if (element is Edge edge)
                    {
                        var inNode = edge.input.node as RedDotNodeView;
                        var outNode = edge.output.node as RedDotNodeView;
                        graphData.RemoveEdge(inNode.GetContext(), outNode.GetContext());
                    }
                    else if (element is RedDotNodeView nodeView)
                    {
                        graphData.RemoveNodeContext(nodeView.GetContext());
                    }
                }
            }
//            Debug.Log(graphViewChange.moveDelta);
            return graphViewChange;
        }
    }
}