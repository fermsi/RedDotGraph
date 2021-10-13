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
        private Image _iconImg;
        private RedDotNodeContext _nodeContext;
        private RedDotGraphData _graphData;

        public RedDotNodeStatusItem(RedDotNodeContextStatus status, RedDotNodeContext nodeContext, RedDotGraphData graphData)
        {
            this._nodeContext = nodeContext;
            this._graphData = graphData;
            var visualTreeNormal = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                PathUtil.GetEditorFilePath("Resources/Styles/NodeIconItem.uxml"));
            VisualElement container = visualTreeNormal.Instantiate();
            Add(container);
            _iconImg = container.Q<Image>("iconImg");
//            iconImg.scaleMode = ScaleMode.ScaleAndCrop;
            var togger = container.Q<Toggle>("toggle");
            togger.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                nodeContext.redDotType += (evt.newValue ? 1 : -1) * (int) status;
                graphData.Dirty();
            });
            SetRedDotStatus(status);
            togger.value = (nodeContext.redDotType & (int)status) > 0;
        }
        
        private void SetRedDotStatus(RedDotNodeContextStatus status)
        {
            var cfg = RedDotEditorConfig.GetStatusConfig(status);
            if (cfg == null)
            {
                Debug.LogError($"{_graphData.keyEnumDict.GetNodeKey(_nodeContext.keyId)}对应的status:{status}配置没有找到，请在RedDotEditorConfig:InitConfig里添加配置");
                return;
            }
            _iconImg.image = Resources.Load<Texture2D>(cfg.Icon);
        }
    }
}