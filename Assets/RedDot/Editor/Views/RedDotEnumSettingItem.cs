using System.Diagnostics;
using RedDot.Editor.Data;
using RedDot.Editor.Util;
using UnityEditor;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace RedDot.Editor.Views
{
    public class RedDotEnumSettingItem : VisualElement
    {
        private RedDotEnumDict _enumDict;
        private int _id;

        private Button deleteBtn;

        private VisualElement otherContainer;
        private TextField keyTextField;
        private Label valueLabel;
        private TextField descTextField;

        public RedDotEnumSettingItem(RedDotEnumDict enumDict)
        {
            _enumDict = enumDict;
//            AssetDatabase.asset
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    PathUtil.GetEditorFilePath("Resources/Styles/SettingItem.uxml"));
            VisualElement container = visualTree.Instantiate();
            Add(container);
            deleteBtn = container.Q<Button>("deleteBtn");
            deleteBtn.RegisterCallback<MouseUpEvent>((evt) =>
            {
                _enumDict.Remove(_id);
            });

            otherContainer = container.Q<VisualElement>("other");
            keyTextField = container.Q<TextField>("keyTextField");
            keyTextField.RegisterValueChangedCallback(evt =>
            {
                _enumDict.ChangeKey(_id, keyTextField.value);
            });
            valueLabel = container.Q<Label>("value");
            descTextField = container.Q<TextField>("descTextField");
            descTextField.RegisterValueChangedCallback(evt =>
            {
                _enumDict.ChangeDesc(_id, descTextField.value);
            });
        }

        public void SetId(int id)
        {
            _id = id;
            otherContainer.visible = true;
            valueLabel.text = $"ID: {id}";
            keyTextField.value = _enumDict.GetKey(_id);
            descTextField.value = _enumDict.GetDesc(_id);
        }
    }
}