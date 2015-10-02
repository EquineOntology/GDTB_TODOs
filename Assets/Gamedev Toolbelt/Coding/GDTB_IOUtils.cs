using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public static class GDTB_IOUtils {

	public static GUISkin GetGUISkin()
	{
        GUISkin skin = AssetDatabase.LoadAssetAtPath(GetFirstInstanceOfFile("GDTBSkin.guiskin"), typeof(GUISkin)) as GUISkin;
        return skin;
    }

	private static string GetFirstInstanceOfFile(string fileName)
	{
        var projectDirectoryPath = Directory.GetCurrentDirectory();
        var projectDirectoryInfo = new DirectoryInfo(projectDirectoryPath);
        var listOfAssetsDirs = projectDirectoryInfo.GetDirectories("Assets");
        var assetsDir = "";
        foreach(var dir in listOfAssetsDirs)
        {
            if (dir.FullName.EndsWith("\\Assets"))
            {
                assetsDir = dir.FullName;
            }
        }
        var path = assetsDir;

        var q = new Queue<string>();
        q.Enqueue(path);
        var absolutePath = "";
        while (q.Count > 0)
        {
            path = q.Dequeue();
            try
            {
                foreach (string subDir in Directory.GetDirectories(path))
                {
                    q.Enqueue(subDir);
                }
            }
            catch (System.Exception ex)
            {
                //Debug.LogError(ex);
            }
            string[] files = null;
            try
            {
                files = Directory.GetFiles(path);
            }
            catch (System.Exception ex)
            {
                //Debug.LogError(ex);
            }
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].EndsWith(fileName))
                    {
                        absolutePath = files[i];
                    }
                }
            }
        }
        var relativePath = absolutePath.Remove(0, projectDirectoryPath.Length + 1);
        //Debug.Log(relativePath);
        return relativePath;
    }
}