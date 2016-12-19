using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace com.immortalhydra.gdtb.codetodos
{
    // If you don't know what an asset postprocessor is, don't worry about this class, it won't change anything.
    // If you know what an asset postprocessor is: I need to use OnPostProcessAllAssets because there's no function
    // for text files only, so I need to actually check if each file is a script to update the QQQ/Scripts db.
    public class ScriptsPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // Remove QQQs from deleted files.
            foreach (var asset in deletedAssets)
            {
                //Debug.Log("deletedassets: " + asset);
                if (asset.EndsWith(".cs") || asset.EndsWith(".js"))
                {
                    QQQOps.RemoveScript(asset);
                }
            }

            // Change the script reference for QQQs when a script is moved.
            for (var i = 0; i < movedAssets.Length; i++)
            {
                //Debug.Log("movedassets: " + movedAssets[i]);
                if (movedAssets[i].EndsWith(".cs") || movedAssets[i].EndsWith(".js"))
                {
                    QQQOps.ChangeScriptOfQQQ(movedFromAssetPaths[i], movedAssets[i]);
                }
            }

            var excludedScripts = IO.GetExcludedScripts();
            var importedAssetsCopy = new List<string>();

            foreach (var asset in importedAssets)
            {
                //Debug.Log("Importedassets: " + asset);
                var shouldBeExcluded = false;
                foreach (var exclusion in excludedScripts)
                {
                    if (asset.Contains(exclusion))
                    {
                        shouldBeExcluded = true;
                    }
                }
                if (shouldBeExcluded == false)
                {
                    importedAssetsCopy.Add(asset);
                }
            }

            // Add QQQs from a script if it was added or reimported (i.e. modified).
            foreach (var asset in importedAssetsCopy)
            {
                //Debug.Log("Importedassetscopy: " + asset);
                if (asset.EndsWith(".cs") || asset.EndsWith(".js"))
                {
                    QQQOps.AddQQQs(asset);

                    if(!QQQOps.AllScripts.Contains(asset))
                    {
                        QQQOps.AllScripts.Add(asset);
                    }
                }
            }

            IO.WriteQQQsToFile();
            WindowMain.WasHiddenByReimport = true;
        }
    }
}