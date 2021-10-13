using System.IO;
using RedDot.Editor.Config;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RedDot.Editor.Importers
{
    [CustomEditor(typeof(RedDotGraphImporter))]
    public class RedDotGraphImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Red Dot Editor"))
            {
                AssetImporter importer = target as AssetImporter;
                ShowGraphEditWindow(importer.assetPath);
            }
            
            ApplyRevertGUI();
        }
        
        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceID);
            return ShowGraphEditWindow(path);
        }
        
        internal static bool ShowGraphEditWindow(string path)
        {
            var guid = AssetDatabase.AssetPathToGUID(path);
            var extension = Path.GetExtension(path);
            if (string.IsNullOrEmpty(extension))
                return false;
            extension = extension.Substring(1).ToLowerInvariant();
            if (extension != RedDotEditorConfig.EXTENSION)
                return false;
            
            foreach (var w in Resources.FindObjectsOfTypeAll<RedDotWindow>())
            {
                if (w.curGuid == guid)
                {
                    w.Focus();
                    return true;
                }
            }

            var window = EditorWindow.CreateWindow<RedDotWindow>(typeof(RedDotWindow));
            window.Initialize(guid);
//            window.Initialize(guid);
            window.Focus();
            return true;
        }
    }
}