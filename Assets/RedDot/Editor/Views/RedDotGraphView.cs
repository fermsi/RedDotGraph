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
        private RedDotWindow _editorWindow;
        private RedDotGraphData _graphData;
        public RedDotGraphView(RedDotWindow editorWindow, RedDotGraphData graphData)
        {
            _graphData = graphData;
            _editorWindow = editorWindow;

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
            if (_editorWindow.keySettingWindow != null)
            {
                _editorWindow.keySettingWindow.Focus();
                return;
            }
            var window = EditorWindow.CreateWindow<RedDotEnumDictWindow>(typeof(RedDotEnumDictWindow), typeof(RedDotWindow));
            window.Initialize(_graphData.keyEnumDict, RedDotEditorConfig.KEY_WINDOW_TITLE);
            window.Focus();
            _editorWindow.keySettingWindow = window;
        }

        private void ExternalIdSetting()
        {
            if (_editorWindow.externalIdSettingWindow != null)
            {
                _editorWindow.externalIdSettingWindow.Focus();
                return;
            }
            var window = EditorWindow.CreateWindow<RedDotEnumDictWindow>(typeof(RedDotEnumDictWindow), typeof(RedDotWindow));
            window.Initialize(_graphData.externalIdEnumDict, RedDotEditorConfig.EXTERNAL_ID_WINDOW_TITLE);
            window.Focus();
            _editorWindow.externalIdSettingWindow = window;
        }

        private void InitElements()
        {
            Dictionary<int, RedDotNodeView> views = new Dictionary<int, RedDotNodeView>();
            foreach (var context in _graphData.nodeContexts)
            {
                var node = new RedDotNodeView(context, _graphData);
                AddElement(node);
                views[context.id] = node;
            }

            foreach (var context in _graphData.nodeContexts)
            {
                var outputNode = views[context.id];
                foreach (var outNodeId in context.outNodeIds)
                {
                    var inputNode = views[outNodeId];
                    outputNode.OutPort.ConnectTo(inputNode.InPort);
                    var edge = new Edge()
                    {
                        output = outputNode.OutPort,
                        input = inputNode.InPort,
                        pickingMode = PickingMode.Position,
                    };
                    AddElement(edge);
                }
                
            }
        }
    
        private bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var type = searchTreeEntry.userData as Type;
            var windowRoot = _editorWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - _editorWindow.position.position);
            var graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);
            
            Node node = Activator.CreateInstance(type, _graphData.CreateNodeContext(graphMousePosition), _graphData) as Node;
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
                    _graphData.Dirty();
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                List<Edge> addFailEdges = new List<Edge>();
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    var inNode = edge.input.node as RedDotNodeView;
                    var outNode = edge.output.node as RedDotNodeView;
                    if (!_graphData.AddEdge(inNode.GetContext(), outNode.GetContext()))
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
                        _graphData.RemoveEdge(inNode.GetContext(), outNode.GetContext());
                    }
                    else if (element is RedDotNodeView nodeView)
                    {
                        _graphData.RemoveNodeContext(nodeView.GetContext());
                    }
                }
            }
//            Debug.Log(graphViewChange.moveDelta);
            return graphViewChange;
        }
    }
}