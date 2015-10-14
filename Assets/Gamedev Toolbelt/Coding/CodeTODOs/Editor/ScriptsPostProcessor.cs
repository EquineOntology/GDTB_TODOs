﻿using UnityEditor;
using System.Collections.Generic;

// If you don't know what an asset postprocessor is, don't worry about this class, it won't change anything.
// If you know what an asset postprocessor is: I need to use OnPostProcessAllAssets because there's no function
// for text files only, so I need to actually check if each file is a script.
public class ScriptsPostProcessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // Remove QQQs from deleted files.
        foreach(var asset in deletedAssets)
        {
            if(asset.EndsWith(".cs") || asset.EndsWith(".js"))
            {
                CodeTODOsHelper.RemoveScript(asset);
            }
        }

        var importedAssetsCopy = new List<string>();
        importedAssetsCopy.AddRange(importedAssets);
        // Change the script reference for QQQs when a script is moved.
        for(int i = 0; i < movedAssets.Length; i++)
        {
            if(movedAssets[i].EndsWith(".cs") || movedAssets[i].EndsWith(".js"))
            {
                CodeTODOsHelper.ChangeScriptOfQQQ(movedFromAssetPaths[i], movedAssets[i]);
            }
        }

        // Add QQQs from a script if it was added or reimported (i.e. modified), but don't create duplicates.
        foreach(var asset in importedAssetsCopy)
        {
            if(asset.EndsWith(".cs") || asset.EndsWith(".js"))
            {
                CodeTODOsHelper.AddQQQs(asset);
            }
        }
    }
}