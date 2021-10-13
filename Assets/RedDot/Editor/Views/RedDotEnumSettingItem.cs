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

        private VisualElement _otherContainer;
        private TextField _keyTextField;
        private Label _valueLabel;
        private TextField _descTextField;

        public RedDotEnumSettingItem(RedDotEnumDict enumDict)
        {
            _enumDict = enumDict;
//            AssetDatabase.asset
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    PathUtil.GetEditorFilePath("Resources/Styles/SettingItem.uxml"));
            VisualElement container = visualTree.Instantiate();
            Add(container);
            var deleteBtn = container.Q<Button>("deleteBtn");
            deleteBtn.RegisterCallback<MouseUpEvent>((evt) =>
            {
                _enumDict.Remove(_id);
            });

            _otherContainer = container.Q<VisualElement>("other");
            _keyTextField = container.Q<TextField>("keyTextField");
            _keyTextField.RegisterValueChangedCallback(evt =>
            {
                _enumDict.ChangeKey(_id, _keyTextField.value);
            });
            _valueLabel = container.Q<Label>("value");
            _descTextField = container.Q<TextField>("descTextField");
            _descTextField.RegisterValueChangedCallback(evt =>
            {
                _enumDict.ChangeDesc(_id, _descTextField.value);
            });
        }

        public void SetId(int id)
        {
            _id = id;
            _otherContainer.visible = true;
            _valueLabel.text = $"ID: {id}";
            _keyTextField.value = _enumDict.GetKey(_id);
            _descTextField.value = _enumDict.GetDesc(_id);
        }
    }
}