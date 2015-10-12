using UnityEngine;
using UnityEditor;

// If you don't know what an asset postprocessor is, don't worry about this class, it won't change anything.
// If you know what an asset postprocessor is: I need to use OnPostProcessAllAssets because there's no function
// for text files only, so I need to actually check if each file is a script.
public class ScriptsPostProcessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // Add QQQs from a script if it was added or reimported (i.e. modified), but don't create duplicates.
        foreach(var asset in importedAssets)
        {
            if(asset.EndsWith(".cs") || asset.EndsWith(".js"))
            {
                var qqqs = CodeTODOsHelper.GetQQQsFromScript(asset);

                for(int i = 0; i < qqqs.Count; i++)
                {
                    var newQQQ = new QQQ(qqqs[i], asset);
                    if (!CodeTODOs.QQQs.Contains(newQQQ))
                    {
                        Debug.Log("Added QQQ");
                        CodeTODOs.QQQs.Add(newQQQ);
                    }
                }
            }
        }

        // Remove QQQs from deleted files.
        foreach(var asset in deletedAssets)
        {
            if(asset.EndsWith(".cs") || asset.EndsWith(".js"))
            {
                CodeTODOsHelper.RemoveScriptFromDB(asset);
            }
        }

        // Change the script reference for QQQs when a script is moved.
        for(int i = 0; i < movedAssets.Length; i++)
        {
            if(movedAssets[i].EndsWith(".cs") || movedAssets[i].EndsWith(".js"))
            {
                CodeTODOsHelper.ChangeScriptOfQQQ(movedFromAssetPaths[i], movedAssets[i]);
            }
        }
    }
}