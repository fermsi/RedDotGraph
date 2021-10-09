using System;
using System.Collections.Generic;
using System.IO;
using RedDot.Editor.Data;
using RedDot.Editor.Util;
using RedDot.Editor.Views;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedDot.Editor
{
    public class RedDotEnumDictWindow : EditorWindow
    {
        private RedDotEnumDict _enumDict;
        private List<int> _ids;
        private ListView _listView;
        public void Initialize(RedDotEnumDict enumDict, string titleStr)
        {
            _enumDict = enumDict;
            rootVisualElement.Clear();
            titleContent = new GUIContent(titleStr);
            var toolbar = new IMGUIContainer(() =>
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                if (GUILayout.Button("增加Key & Value条目", EditorStyles.toolbarButton))
                {
                    enumDict.Add();
                }
                GUILayout.Space(6);
                if (GUILayout.Button("生成代码", EditorStyles.toolbarButton))
                {;
                    ExportTo();
                }
                
                GUILayout.EndHorizontal();
            });
            rootVisualElement.Add(toolbar);
            InitListView();
            
            var exportFileNameField = new TextField("生成文件名");
            exportFileNameField.value = _enumDict.exportFileName;
            exportFileNameField.RegisterValueChangedCallback(evt =>
            {
                _enumDict.ChangeExportFileName(exportFileNameField.value);
            });
            rootVisualElement.Add(exportFileNameField);
            
            var keyValueRegField = new TextField("Key & Value生成规则");
            keyValueRegField.multiline = true;
            keyValueRegField.value = _enumDict.keyValueReg;
            keyValueRegField.RegisterValueChangedCallback(evt =>
            {
                _enumDict.ChangeKeyAndValueReg(keyValueRegField.value);
            });
            rootVisualElement.Add(keyValueRegField);
            
            var templateField = new TextField("代码模板");
            templateField.multiline = true;
            templateField.value = _enumDict.classTemplate;
            templateField.RegisterValueChangedCallback(evt =>
            {
                _enumDict.ChangeClassTemplate(templateField.value);
            });
            rootVisualElement.Add(templateField);
            
            RefreshData();
        }

        public void RefreshData()
        {
            if (_enumDict == null) return;
            _ids = _enumDict.GetIds();
            _listView.itemsSource = _ids;
            _listView.ScrollToItem(_ids.Count - 1);
        }

        private void InitListView()
        {
            Func<VisualElement> makeItem = () =>
            {
                return new RedDotEnumSettingItem(_enumDict);
            };

            Action<VisualElement, int> bindItem = (e, i) => (e as RedDotEnumSettingItem).SetId(_ids[i]);

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PathUtil.GetEditorFilePath("Resources/Styles/ListView.uxml"));
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PathUtil.GetEditorFilePath("Resources/Styles/ListView.uss"));
            VisualElement container = visualTree.Instantiate();
            rootVisualElement.Add(container);
            container.styleSheets.Add(styleSheet);
            _listView = container.Q<ListView>();
            _listView.makeItem = makeItem;
            _listView.bindItem = bindItem;
            _listView.selectionType = SelectionType.None;
            _listView.showBorder = true;
        }

//        void OnEnable()
//        {
//            RefreshData();
//        }

        private void Update()
        {
            if (_enumDict.IsChanged)
            {
                _enumDict.ClearChangedTag();
                RefreshData();
            }
        }
        
        public void ExportTo()
        {
            if (_enumDict != null)
            {
                var oldFilePath = Application.dataPath;
                var extension = Path.GetExtension(_enumDict.exportFileName);
                extension = extension.Replace(".", "");
                Debug.Log($"{_enumDict.exportFileName}, {Path.GetFileNameWithoutExtension(_enumDict.exportFileName)}, {Path.GetExtension(_enumDict.exportFileName)}");
                var newFilePath = EditorUtility.SaveFilePanelInProject("Export Red Dot Graph to Json", Path.GetFileNameWithoutExtension(_enumDict.exportFileName), extension, "", oldFilePath);
                newFilePath = newFilePath.Replace(Application.dataPath, "Assets");

                if (!string.IsNullOrEmpty(newFilePath))
                {
                    _enumDict.ExportTo(newFilePath);
                }
            }
        }
    }
}