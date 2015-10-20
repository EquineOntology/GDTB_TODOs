using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public static class GDTB_IOUtils
{
    // Return the path to the extension's folder.
    public static string GetGDTBPath()
    {
        var path = GetFirstInstanceOfFolder("Gamedev Toolbelt");
        return path;
    }

    // Return the first instance of the given filename.
    // This is a non-recursive, breadth-first search algorithm.
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
            catch (System.Exception /*ex*/)
            {
                //Debug.LogError(ex);
            }
            string[] files = null;
            try
            {
                files = Directory.GetFiles(path);
            }
            catch (System.Exception /*ex*/)
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

    // Return the first instance of the given folder.
    // This is a non-recursive, breadth-first search algorithm.
    private static string GetFirstInstanceOfFolder(string folderName)
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
            catch (System.Exception /*ex*/)
            {
                //Debug.LogError(ex);
            }
            string[] folders = null;
            try
            {
                folders = Directory.GetDirectories(path);
            }
            catch (System.Exception /*ex*/)
            {
                //Debug.LogError(ex);
            }
            if (folders != null)
            {
                for (int i = 0; i < folders.Length; i++)
                {
                    if (folders[i].EndsWith(folderName))
                    {
                        absolutePath = folders[i];
                    }
                }
            }
        }
        var relativePath = absolutePath.Remove(0, projectDirectoryPath.Length + 1);
        //Debug.Log(relativePath);
        return relativePath;
    }

    // Remove a single line from a text file.
    public static void RemoveLineFromFile(string file, int lineNumber)
    {
        var tempFile = Path.GetTempFileName();

        using(var reader = new StreamReader(file))
        using (var writer = new StreamWriter(tempFile))
        {
            string line;
            int currentLineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                // If the line is not the one we want to remove, write it to the temp file.
                if (currentLineNumber != lineNumber)
                {
                    writer.WriteLine(line);
                }
                else
                {
                    var lineWithoutQQQ = GetLineWithoutQQQ(line);
                    if (!System.String.IsNullOrEmpty(lineWithoutQQQ))
                    {
                        writer.WriteLine(lineWithoutQQQ);
                    }
                }
                currentLineNumber++;
            }
        }

        // Overwrite the old file with the temp file.
        File.Delete(file);
        File.Move(tempFile, file);
    }

    // Check for character before the QQQ to see if they are spaces or backslashes. If they are, remove them.
    // This is to remove the whole QQQ wihtout removing anything else of importance (including stuff in a comment BEFORE a QQQ).
    private static string GetLineWithoutQQQ(string line)
    {
        var qqqIndex = line.IndexOf(CodeTODOsPrefs.TODOToken);

        int j = qqqIndex - 1;
        while (j >= 0 && (line[j] == ' ' || line[j] == '/'))
        {
            if (j > 0)
            {
                j--;
                qqqIndex--;
            }
            else
            {
                return null;
            }
        }
        var lineWithoutQQQ = line.Substring(0, line.Length - (line.Length - qqqIndex));

        return lineWithoutQQQ;
    }
}