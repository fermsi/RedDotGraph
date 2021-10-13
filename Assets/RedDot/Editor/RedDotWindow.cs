using System;
using System.IO;
using System.Text;
using RedDot.Editor.Config;
using RedDot.Editor.Data;
using RedDot.Editor.Importers;
using RedDot.Editor.Util;
using RedDot.Editor.Views;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RedDot.Editor
{
    public class RedDotWindow : EditorWindow
    {
        private RedDotGraphView _graphView;
        private RedDotGraphData _graphData;
        public string curGuid;

        public RedDotEnumDictWindow keySettingWindow;
        public RedDotEnumDictWindow externalIdSettingWindow;

        private bool lastDirtyState;
        private bool assetMaybeDeleted;
        private bool assetMaybeChangedOnDisk;

        public void Initialize(string assetGuid)
        {
            try
            {
//                Debug.Log($"=={assetGuid}==init");
                EditorApplication.wantsToQuit -= PromptSaveIfDirtyOnQuit;
                var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(assetGuid));
                if (asset == null)
                    return;

                if (!EditorUtility.IsPersistent(asset))
                    return;

                if (curGuid == assetGuid)
                    return;

                var path = AssetDatabase.GetAssetPath(asset);
                var extension = Path.GetExtension(path);
                if (extension == null)
                    return;
                extension = extension.Substring(1).ToLowerInvariant();
                if (extension != RedDotEditorConfig.EXTENSION)
                {
                    return;
                }
                curGuid = assetGuid;
                
                var scriptObj = MonoScript.FromScriptableObject(this);
                PathUtil.CurEditorPath = AssetDatabase.GetAssetPath(scriptObj).Replace("RedDotWindow.cs", "");
//                Debug.Log($"======PathUtil.CurEditorPath:{PathUtil.CurEditorPath}");
            
                var textGraph = File.ReadAllText(path, Encoding.UTF8);
                _graphData = JsonUtility.FromJson<RedDotGraphData>(textGraph);
                _graphData.ValidateGraph();
                UpdateTitle();
                _graphView = new RedDotGraphView(this, _graphData);
                rootVisualElement.Add(_graphView);
                Repaint();

                EditorApplication.wantsToQuit += PromptSaveIfDirtyOnQuit;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void OnFocus()
        {
            if (_graphData == null) return;
            if (!_graphData.IsDataCached())
            {
                _graphData.CacheData();
                RepaintGraph();
            }
        }

        private void Update()
        {
            if (_graphData == null) return;
            if (_graphData.IsDirty != lastDirtyState)
            {
                UpdateTitle();
            }

            if (assetMaybeChangedOnDisk)
            {
                RepaintGraph();
                assetMaybeChangedOnDisk = false;
            }
        }

        public void AssetWasDeleted()
        {
//            Debug.Log("AssetWasDeleted");
            assetMaybeDeleted = true;
            UpdateTitle();
        }

        /// <summary>
        /// 文件被改了
        /// </summary>
        public void CheckForChanges()
        {
//            Debug.Log("CheckForChanges");
            if (!assetMaybeDeleted && _graphData != null)
            {
                assetMaybeChangedOnDisk = true;
                UpdateTitle();
            }
        }
        
        bool AssetFileExists()
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(curGuid);
            return File.Exists(assetPath);
        }
        public void UpdateTitle()
        {
            lastDirtyState = _graphData.IsDirty;
            string assetPath = AssetDatabase.GUIDToAssetPath(curGuid);
            string assetName = Path.GetFileNameWithoutExtension(assetPath);
            string title = assetName;
            if (_graphData.IsDirty)
            {
                title = title + "*";
            }

            if (!AssetFileExists())
            {
                title = title + " (deleted)";
            }
            Texture2D icon;
            {
                icon = Resources.Load<Texture2D>("Icons/icon_reddot_title");
            }
            titleContent = new GUIContent(title, icon);
        }

        void RepaintGraph()
        {
            if (_graphView != null)
            {
                _graphView.parent.Remove(_graphView);
                _graphView = null;
            }
            if (_graphData != null)
            {
                _graphData.ValidateGraph();
                _graphView = new RedDotGraphView(this, _graphData);
                rootVisualElement.Add(_graphView);
                Repaint();

                if (keySettingWindow != null)
                {
                    keySettingWindow.Initialize(_graphData.keyEnumDict, RedDotEditorConfig.KEY_WINDOW_TITLE);
                }

                if (externalIdSettingWindow != null)
                {
                    externalIdSettingWindow.Initialize(_graphData.externalIdEnumDict, RedDotEditorConfig.EXTERNAL_ID_WINDOW_TITLE);
                }
            }
        }
   
        void OnEnable()
        {
//            Debug.Log($"=={curGuid}==OnEnable0");
            // subscribe this event so it is registered on assembly reloads (we don't run Initialize on assembly reload)
            // doing remove before add ensures we never subscribe twice
            EditorApplication.wantsToQuit -= PromptSaveIfDirtyOnQuit;

            RepaintGraph();
            
            EditorApplication.wantsToQuit += PromptSaveIfDirtyOnQuit;
        }

        void OnDisable()
        {
//            Debug.Log($"=={curGuid}==OnDisable1");
            _graphView = null;
            _graphData.CacheData();
            EditorApplication.wantsToQuit -= PromptSaveIfDirtyOnQuit;
        }
        
        void OnDestroy()
        {
//            Debug.Log("====OnDestroy");
            RedDotWindow newWindow = null;
            if (!PromptSaveIfDirtyOnQuit())
            {
                newWindow = CreateWindow<RedDotWindow>(typeof(RedDotWindow));//, typeof(SceneView));
                newWindow.Initialize(curGuid);
            }
            else
            {
                // the window is closing for good.. cleanup undo history for the graph object
//                Undo.ClearUndo(graphObject);
            }

//            graphObject = null;
//            graphEditorView = null;

            // show new window if we have one
            if (newWindow != null)
            {
                newWindow.Show();
                newWindow.Focus();
            }

            if (keySettingWindow != null)
            {
                keySettingWindow.Close();
                keySettingWindow = null;
            }

            if (externalIdSettingWindow != null)
            {
                externalIdSettingWindow.Close();
                externalIdSettingWindow = null;
            }
        }
        
        // returns the asset path the file was saved to, or NULL if nothing was saved
        string SaveAsImplementation(bool openWhenSaved)
        {
            string savedFilePath = null;

            if (curGuid != null && _graphData != null)
            {
                var oldFilePath = AssetDatabase.GUIDToAssetPath(curGuid);
                if (string.IsNullOrEmpty(oldFilePath) || _graphData == null)
                    return null;

                // The asset's name needs to be removed from the path, otherwise SaveFilePanel assumes it's a folder
                string oldDirectory = Path.GetDirectoryName(oldFilePath);

                var extension = RedDotEditorConfig.EXTENSION;
                var newFilePath = EditorUtility.SaveFilePanelInProject("Save Red Dot Graph As...", Path.GetFileNameWithoutExtension(oldFilePath), extension, "", oldDirectory);
                newFilePath = newFilePath.Replace(Application.dataPath, "Assets");

                if (newFilePath != oldFilePath)
                {
                    if (!string.IsNullOrEmpty(newFilePath))
                    {
                        bool success = (FileUtilities.WriteRedDotGraphDataToDisk(newFilePath, _graphData) != null);
                        AssetDatabase.ImportAsset(newFilePath);
                        if (success)
                        {
                            if (openWhenSaved)
                                RedDotGraphImporterEditor.ShowGraphEditWindow(newFilePath);
                            savedFilePath = newFilePath;
                        }
                    }
                }
                else
                {
                    // saving to the current path
                    if (SaveAsset())
                    {
                        savedFilePath = oldFilePath;
                    }
                }
            }
            return savedFilePath;
        }
        
        // returns true when the graph has been successfully saved, or the user has indicated they are ok with discarding the local graph
        // returns false when saving has failed
        bool DisplayDeletedFromDiskDialog(bool reopen = true)
        {
            // first double check if we've actually been deleted
            bool saved = false;
            bool okToClose = false;
            string originalAssetPath = AssetDatabase.GUIDToAssetPath(curGuid);

            while (true)
            {
                int option = EditorUtility.DisplayDialogComplex(
                    "Graph removed from project",
                    "The file has been deleted or removed from the project folder.\n\n" +
                    originalAssetPath +
                    "\n\nWould you like to save your Graph Asset?",
                    "Save As...", "Cancel", "Discard Graph and Close Window");

                if (option == 0)
                {
                    string savedPath = SaveAsImplementation(false);
                    if (savedPath != null)
                    {
                        saved = true;

                        // either close or reopen the local window editor
                        _graphData = null;
                        curGuid = (reopen ? AssetDatabase.AssetPathToGUID(savedPath) : null);

                        break;
                    }
                }
                else if (option == 2)
                {
                    okToClose = true;
                    _graphData = null;
                    curGuid = null;
                    break;
                }
                else if (option == 1)
                {
                    // continue in deleted state...
                    break;
                }
            }

            return (saved || okToClose);
        }
        
        private bool PromptSaveIfDirtyOnQuit()
        {
            // only bother unless we've actually got data to preserve
            if (_graphData != null)
            {
                // if the asset has been deleted, ask the user what to do
                if (!AssetFileExists())
                    return DisplayDeletedFromDiskDialog(false);

                // if there are unsaved modifications, ask the user what to do
//                Debug.Log($"------PromptSaveIfDirtyOnQuit---{_graphData.IsDirty}");
                if (_graphData.IsDirty)
                {
                    int option = EditorUtility.DisplayDialogComplex(
                        "Red Dot Graph Has Been Modified",
                        "Do you want to save the changes you made in the Red Dot Graph?\n\n" +
                        AssetDatabase.GUIDToAssetPath(curGuid) +
                        "\n\nYour changes will be lost if you don't save them.",
                        "Save", "Cancel", "Discard Changes");

                    if (option == 0) // save
                    {
                        return SaveAsset();
                    }
                    else if (option == 1) // cancel (or escape/close dialog)
                    {
                        return false;
                    }
                    else if (option == 2) // discard
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        private bool SaveAsset()
        {
            var pathName = AssetDatabase.GUIDToAssetPath(curGuid);
            return _graphData.SaveTo(pathName);
        }

        public void CheckAndSave()
        {
            if (_graphData != null)
            {
                if (!AssetFileExists())
                {
                    string originalAssetPath = AssetDatabase.GUIDToAssetPath(curGuid);

                    while (true)
                    {
                        int option = EditorUtility.DisplayDialogComplex(
                            "Graph removed from project",
                            "The file has been deleted or removed from the project folder.\n\n" +
                            originalAssetPath +
                            "\n\nWould you like to save your Graph Asset?",
                            "Save As...", "Cancel", "Discard Graph and Close Window");

                        if (option == 0)
                        {
                            string savedPath = SaveAsImplementation(false);
                            if (savedPath != null)
                            {
                                curGuid = AssetDatabase.AssetPathToGUID(savedPath);
                                break;
                            }
                        }
                        else if (option == 2)
                        {
                            _graphData = null;
                            curGuid = null;
                            Close();
                            return;
                        }
                        else if (option == 1)
                        {
                            // continue in deleted state...
                            break;
                        }
                    }
                }

                SaveAsset();
            }
        }
        
        public void ExportTo()
        {
            if (curGuid != null && _graphData != null)
            {
                var oldFilePath = AssetDatabase.GUIDToAssetPath(curGuid);
                if (string.IsNullOrEmpty(oldFilePath) || _graphData == null)
                {
                    Debug.LogError("Export error");
                    return;
                }
                string oldDirectory = Path.GetDirectoryName(oldFilePath);

                var extension = RedDotEditorConfig.EXPORT_EXTENSION;
                var newFilePath = EditorUtility.SaveFilePanelInProject("Export Red Dot Graph to Json", Path.GetFileNameWithoutExtension(oldFilePath), extension, "", oldDirectory);
                newFilePath = newFilePath.Replace(Application.dataPath, "Assets");

                if (!string.IsNullOrEmpty(newFilePath))
                {
                    _graphData.ExportTo(newFilePath);
                }
            }
        }
    }
}