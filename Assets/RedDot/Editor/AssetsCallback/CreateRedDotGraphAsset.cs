using RedDot.Editor.Config;
using UnityEditor;
using UnityEngine;

namespace RedDot.Editor.AssetsCallback
{
    public static class CreateRedDotGraphAsset
    {
        [MenuItem("Assets/Create/Red Dot Graph", false, 9999)]
        public static void CreateRedDotGraph()
        {
            var graphItem = ScriptableObject.CreateInstance<NewRedDotGraphAction>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, graphItem,
                $"New Red Dot Graph.{RedDotEditorConfig.EXTENSION}", null, null);
        }
    }
}