using System;
using System.Linq;
using RedDot.Editor.Config;
using UnityEditor;
using UnityEngine;

namespace RedDot.Editor.Importers
{
    class RedDotGraphAssetPostProcessor : AssetPostprocessor
    {
        static void UpdateAfterAssetChange(string[] newNames)
        {
            // This will change the title of the window.
            RedDotWindow[] windows = Resources.FindObjectsOfTypeAll<RedDotWindow>();
            foreach (var matGraphEditWindow in windows)
            {
                for (int i = 0; i < newNames.Length; ++i)
                {
                    if (matGraphEditWindow.curGuid == AssetDatabase.AssetPathToGUID(newNames[i]))
                        matGraphEditWindow.UpdateTitle();
                }
            }
        }

        static void DisplayDeletionDialog(string[] deletedAssets)
        {
            RedDotWindow[] windows = Resources.FindObjectsOfTypeAll<RedDotWindow>();
            foreach (var matGraphEditWindow in windows)
            {
                for (int i = 0; i < deletedAssets.Length; ++i)
                {
                    if (matGraphEditWindow.curGuid == AssetDatabase.AssetPathToGUID(deletedAssets[i]))
                        matGraphEditWindow.AssetWasDeleted();
                }
            }
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
//            RegisterShaders(importedAssets);

            // Moved assets
            bool anyMovedShaders = movedAssets.Any(val => val.EndsWith(RedDotEditorConfig.Extension, StringComparison.InvariantCultureIgnoreCase));
            anyMovedShaders |= movedAssets.Any(val => val.EndsWith(RedDotEditorConfig.Extension, StringComparison.InvariantCultureIgnoreCase));
            if (anyMovedShaders)
                UpdateAfterAssetChange(movedAssets);

            // Deleted assets
            bool anyRemovedShaders = deletedAssets.Any(val => val.EndsWith(RedDotEditorConfig.Extension, StringComparison.InvariantCultureIgnoreCase));
            anyRemovedShaders |= deletedAssets.Any(val => val.EndsWith(RedDotEditorConfig.Extension, StringComparison.InvariantCultureIgnoreCase));
            if (anyRemovedShaders)
                DisplayDeletionDialog(deletedAssets);

            var windows = Resources.FindObjectsOfTypeAll<RedDotWindow>();

            var changedGraphGuids = importedAssets
                .Where(x => x.EndsWith(RedDotEditorConfig.Extension, StringComparison.InvariantCultureIgnoreCase)
                    || x.EndsWith(RedDotEditorConfig.Extension, StringComparison.InvariantCultureIgnoreCase))
                .Select(AssetDatabase.AssetPathToGUID)
                .ToList();
            foreach (var window in windows)
            {
                if (changedGraphGuids.Contains(window.curGuid))
                {
                    window.CheckForChanges();
                }
            }
        }
    }
}
