using RedDot.Editor.Config;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace RedDot.Editor.Importers
{
    [ScriptedImporter(1, RedDotEditorConfig.EXTENSION)]
    public class RedDotGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var mainObject = new GameObject("RedDotImportAsset");
            Texture2D texture = Resources.Load<Texture2D>("Icons/icon_reddot_title");
            ctx.AddObjectToAsset("RedDotImportAsset", mainObject, texture);
            ctx.SetMainObject(mainObject);
        }
    }
}