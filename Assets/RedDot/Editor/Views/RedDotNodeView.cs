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
        public Port inPort;
        public Port outPort;
        private RedDotNodeContext nodeContext;
        private RedDotGraphData graphData;
        public RedDotNodeView(RedDotNodeContext nodeContext, RedDotGraphData graphData)
        {
            this.nodeContext = nodeContext;
            this.graphData = graphData;
            //创建一个inputPort
            inPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(Port));
            //设置port显示的名称
            inPort.portName = " ";
            inPort.portColor = Color.red;
            //添加到inputContainer容器中
            inputContainer.Add(inPort);
            outPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(Port));
            outPort.portName = "子节点";
            outPort.portColor = Color.red;
            outputContainer.Add(outPort);
            SetPosition(new Rect(nodeContext.Position.x, nodeContext.Position.y, 0, 0));
            RepaintElements();
            RefreshExpandedState();
        }

        private Image iconImg;
        private void RepaintElements()
        {
            var temp = new VisualElement();
            Add(temp);
            var root = temp;
            var rootStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PathUtil.GetEditorFilePath("Resources/Styles/Node.uss"));
            root.styleSheets.Add(rootStyleSheet);
            var labelStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PathUtil.GetEditorFilePath("Resources/Styles/Label.uss"));
            
            title = graphData.KeyEnumDict.GetNodeKey(nodeContext.KeyId);

            var label = new Label("红点key");
            label.styleSheets.Add(labelStyleSheet);
            root.Add(label);
            
            var choices = graphData.KeyEnumDict.GetNodeKeys();
            var normalField = new PopupField<string>(choices, 0);
            normalField.value = title;
            root.Add(normalField);
            normalField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                nodeContext.KeyId = graphData.KeyEnumDict.GetIdByNodeKey(evt.newValue);
                graphData.Dirty();
                title = evt.newValue;
            });
            
            label = new Label("红点逻辑ID");
            label.styleSheets.Add(labelStyleSheet);
            root.Add(label);
            
            var funcKeys = graphData.ExternalIdEnumDict.GetNodeKeys();
            var funcField = new PopupField<string>(funcKeys, 0);
            funcField.value = graphData.ExternalIdEnumDict.GetNodeKey(nodeContext.FuncId);
            root.Add(funcField);
            funcField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                nodeContext.FuncId = graphData.ExternalIdEnumDict.GetIdByNodeKey(evt.newValue);
                graphData.Dirty();
            });

            
            label = new Label("红点可存在的状态");
            label.styleSheets.Add(labelStyleSheet);
            root.Add(label);
            
            var visualTreeNormal = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                PathUtil.GetEditorFilePath("Resources/Styles/NodeIconItemContainer.uxml"));
            VisualElement container = visualTreeNormal.Instantiate();
            root.Add(container);
            var iconsContainer = container.Q<VisualElement>("container");
            var statusItem = new RedDotNodeStatusItem(RedDotNodeContextStatus.NORMAL, nodeContext, graphData);
            iconsContainer.Add(statusItem);
            statusItem = new RedDotNodeStatusItem(RedDotNodeContextStatus.NEW, nodeContext, graphData);
            iconsContainer.Add(statusItem);

            visualTreeNormal =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    PathUtil.GetEditorFilePath("Resources/Styles/NodeIconItemContainer.uxml"));
            container = visualTreeNormal.Instantiate();
            root.Add(container);
            iconsContainer = container.Q<VisualElement>("container");
            statusItem = new RedDotNodeStatusItem(RedDotNodeContextStatus.NUMBER, nodeContext, graphData);
            iconsContainer.Add(statusItem);
            statusItem = new RedDotNodeStatusItem(RedDotNodeContextStatus.FULL, nodeContext, graphData);
            iconsContainer.Add(statusItem);
        }

        public void NodeChanged()
        {
            nodeContext.Position = GetPosition().position;
        }

        public RedDotNodeContext GetContext()
        {
            return nodeContext;
        }
    }
}