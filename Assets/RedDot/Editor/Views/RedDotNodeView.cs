using System.Collections.Generic;
using RedDot.Editor.Config;
using RedDot.Editor.Data;
using RedDot.Editor.Util;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedDot.Editor.Views
{
    public class RedDotNodeView:Node
    {
        public Port InPort;
        public Port OutPort;
        private RedDotNodeContext _nodeContext;
        private RedDotGraphData _graphData;
        public RedDotNodeView(RedDotNodeContext nodeContext, RedDotGraphData graphData)
        {
            _nodeContext = nodeContext;
            _graphData = graphData;
            //创建一个inputPort
            InPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
            //设置port显示的名称
            InPort.portName = " ";
            InPort.portColor = Color.red;
            //添加到inputContainer容器中
            inputContainer.Add(InPort);
            OutPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(Port));
            OutPort.portName = "子节点";
            OutPort.portColor = Color.red;
            outputContainer.Add(OutPort);
            SetPosition(new Rect(nodeContext.position.x, nodeContext.position.y, 0, 0));
            RepaintElements();
            RefreshExpandedState();
        }

        private void RepaintElements()
        {
            var temp = new VisualElement();
            Add(temp);
            var root = temp;
            var rootStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PathUtil.GetEditorFilePath("Resources/Styles/Node.uss"));
            root.styleSheets.Add(rootStyleSheet);
            var labelStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PathUtil.GetEditorFilePath("Resources/Styles/Label.uss"));
            
            title = _graphData.keyEnumDict.GetNodeKey(_nodeContext.keyId);

            var label = new Label("红点key");
            label.styleSheets.Add(labelStyleSheet);
            root.Add(label);
            
            var choices = _graphData.keyEnumDict.GetNodeKeys();
            var normalField = new PopupField<string>(choices, 0);
            normalField.value = title;
            root.Add(normalField);
            normalField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                _nodeContext.keyId = _graphData.keyEnumDict.GetIdByNodeKey(evt.newValue);
                _graphData.Dirty();
                title = evt.newValue;
            });
            
            label = new Label("红点逻辑ID");
            label.styleSheets.Add(labelStyleSheet);
            root.Add(label);
            
            var funcKeys = _graphData.externalIdEnumDict.GetNodeKeys();
            var funcField = new PopupField<string>(funcKeys, 0);
            funcField.value = _graphData.externalIdEnumDict.GetNodeKey(_nodeContext.funcId);
            root.Add(funcField);
            funcField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                _nodeContext.funcId = _graphData.externalIdEnumDict.GetIdByNodeKey(evt.newValue);
                _graphData.Dirty();
            });

            
            label = new Label("红点可存在的状态");
            label.styleSheets.Add(labelStyleSheet);
            root.Add(label);
            
            var visualTreeNormal = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                PathUtil.GetEditorFilePath("Resources/Styles/NodeIconItemContainer.uxml"));
            VisualElement container = visualTreeNormal.Instantiate();
            root.Add(container);
            var iconsContainer = container.Q<VisualElement>("container");
            var statusItem = new RedDotNodeStatusItem(RedDotNodeContextStatus.Normal, _nodeContext, _graphData);
            iconsContainer.Add(statusItem);
            statusItem = new RedDotNodeStatusItem(RedDotNodeContextStatus.New, _nodeContext, _graphData);
            iconsContainer.Add(statusItem);

            visualTreeNormal =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    PathUtil.GetEditorFilePath("Resources/Styles/NodeIconItemContainer.uxml"));
            container = visualTreeNormal.Instantiate();
            root.Add(container);
            iconsContainer = container.Q<VisualElement>("container");
            statusItem = new RedDotNodeStatusItem(RedDotNodeContextStatus.Number, _nodeContext, _graphData);
            iconsContainer.Add(statusItem);
            statusItem = new RedDotNodeStatusItem(RedDotNodeContextStatus.Full, _nodeContext, _graphData);
            iconsContainer.Add(statusItem);
        }

        public void NodeChanged()
        {
            _nodeContext.position = GetPosition().position;
        }

        public RedDotNodeContext GetContext()
        {
            return _nodeContext;
        }
    }
}