using RedDot.Editor.Config;
using RedDot.Editor.Data;
using RedDot.Editor.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedDot.Editor.Views
{
    public class RedDotNodeStatusItem : VisualElement
    {
        private Image iconImg;
        private RedDotNodeContext nodeContext;
        private RedDotGraphData graphData;

        public RedDotNodeStatusItem(RedDotNodeContextStatus status, RedDotNodeContext nodeContext, RedDotGraphData graphData)
        {
            this.nodeContext = nodeContext;
            this.graphData = graphData;
            var visualTreeNormal = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                PathUtil.GetEditorFilePath("Resources/Styles/NodeIconItem.uxml"));
            VisualElement container = visualTreeNormal.Instantiate();
            Add(container);
            iconImg = container.Q<Image>("iconImg");
//            iconImg.scaleMode = ScaleMode.ScaleAndCrop;
            var togger = container.Q<Toggle>("toggle");
            togger.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                nodeContext.RedDotType += (evt.newValue ? 1 : -1) * (int) status;
                graphData.Dirty();
            });
            SetRedDotStatus(status);
            togger.value = (nodeContext.RedDotType & (int)status) > 0;
        }
        
        private void SetRedDotStatus(RedDotNodeContextStatus status)
        {
            var cfg = RedDotEditorConfig.GetStatusConfig(status);
            if (cfg == null)
            {
                Debug.LogError($"{graphData.KeyEnumDict.GetNodeKey(nodeContext.KeyId)}对应的status:{status}配置没有找到，请在RedDotEditorConfig:InitConfig里添加配置");
                return;
            }
            iconImg.image = Resources.Load<Texture2D>(cfg.Icon);
        }
    }
}