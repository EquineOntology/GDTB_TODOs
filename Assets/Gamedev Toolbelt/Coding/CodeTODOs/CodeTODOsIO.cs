#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;

public static class CodeTODOsIO
{
    /// Return the path to the extension's folder.
    public static string GetGDTBPath()
    {
        var path = GetFirstInstanceOfFolder("Gamedev Toolbelt");
        return path;
    }


    /// Return the first instance of the given filename.
    /// This is a non-recursive, breadth-first search algorithm.
	private static string GetFirstInstanceOfFile(string aFileName)
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
                    if (files[i].EndsWith(aFileName))
                    {
                        absolutePath = files[i];
                    }
                }
            }
        }
        var relativePath = absolutePath.Remove(0, projectDirectoryPath.Length + 1);
        return relativePath;
    }


    /// Return the first instance of the given folder.
    /// This is a non-recursive, breadth-first search algorithm.
    private static string GetFirstInstanceOfFolder(string aFolderName)
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
                    if (folders[i].EndsWith(aFolderName))
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


    /// Remove a single line from a text file.
    public static void RemoveLineFromFile(string aFile, int aLineNumber)
    {
        var tempFile = Path.GetTempFileName();

        using(var reader = new StreamReader(aFile))
        using (var writer = new StreamWriter(tempFile))
        {
            string line;
            int currentLineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                // If the line is not the one we want to remove, write it to the temp file.
                if (currentLineNumber != aLineNumber)
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
        File.Delete(aFile);
        File.Move(tempFile, aFile);
    }


    /// Check for character before the QQQ to see if they are spaces or backslashes. If they are, remove them.
    /// This is to remove the whole QQQ wihtout removing anything else of importance (including stuff in a comment BEFORE a QQQ).
    private static string GetLineWithoutQQQ(string aLine)
    {
        var qqqIndex = aLine.IndexOf(CodeTODOsPrefs.TODOToken);

        int j = qqqIndex - 1;
        while (j >= 0 && (aLine[j] == ' ' || aLine[j] == '/'))
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
        var lineWithoutQQQ = aLine.Substring(0, aLine.Length - (aLine.Length - qqqIndex));

        return lineWithoutQQQ;
    }


    /// Update the task and priority of a QQQ.
    public static void ChangeQQQ(QQQ anOldQQQ, QQQ aNewQQQ)
    {
        var tempFile = Path.GetTempFileName();

        using(var reader = new StreamReader(anOldQQQ.Script))
        using (var writer = new StreamWriter(tempFile))
        {
            string line;
            int currentLineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                // If the line is not the one we want to remove, write it to the temp file.
                if (currentLineNumber != anOldQQQ.LineNumber)
                {
                    writer.WriteLine(line);
                }
                else
                {
                    // Remove the old QQQ and add the new one, then write the line to file.
                    var lineWithoutQQQ = GetLineWithoutQQQ(line);

                    var slashes = "";
                    slashes = string.IsNullOrEmpty(lineWithoutQQQ) ? "//" : " //";

                    var newLine = lineWithoutQQQ + slashes + CodeTODOsPrefs.TODOToken + (((int)aNewQQQ.Priority) + 1) + " " + aNewQQQ.Task;
                    writer.WriteLine(newLine);
                }
                currentLineNumber++;
            }
        }
        // Overwrite the old file with the temp file.
        File.Delete(anOldQQQ.Script);
        File.Move(tempFile, anOldQQQ.Script);
    }


    /// Add a QQQ to a script.
    public static void AddQQQ(QQQ aQQQ)
    {
        var tempFile = Path.GetTempFileName();

        using(var reader = new StreamReader(aQQQ.Script))
        using (var writer = new StreamWriter(tempFile))
        {
            string line;
            int currentLineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                // Add the new QQQ as the first line in the file.
                if (currentLineNumber == aQQQ.LineNumber)
                {
                    var newQQQ = "//QQQ" + (int)aQQQ.Priority + " " + aQQQ.Task;
                    writer.WriteLine(newQQQ);
                }
                writer.WriteLine(line);
                currentLineNumber++;
            }
        }
        // Overwrite the old file with the temp file.
        File.Delete(aQQQ.Script);
        File.Move(tempFile, aQQQ.Script);
    }
}
#endif