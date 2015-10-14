using UnityEditor;
using System.Collections.Generic;
using System.IO;

public static class CodeTODOsHelper
{
    public static List<string> FindAllScripts()
    {
        var allAssets = AssetDatabase.GetAllAssetPaths();
        var allScripts = new List<string>();

        foreach (var path in allAssets)
        {
            if (path.EndsWith(".cs") || path.EndsWith(".js"))
            {
                allScripts.Add(path);
            }
        }
        return allScripts;
    }

    public static void GetQQQsFromAllScripts()
    {
        // We collect all scripts in the project, and then we check them for QQQs.
        var allScripts = FindAllScripts();
        var qqqs = new List<QQQ>();

        for (int i = 0; i < allScripts.Count; i++)
        {
            qqqs.AddRange(GetQQQsFromScript(allScripts[i]));
        }
        CodeTODOs.QQQs = qqqs;
    }

    public static List<QQQ> GetQQQsFromScript(string path)
    {
        var currentQQQs = new List<QQQ>();

        // Since the string "QQQ" is repeated many times in the files listed, its default value would give
        // a bunch of false positives in these files, so we exclude them.
        if (path.EndsWith("CodeTODOs.cs") ||
            path.EndsWith("CodeTODOsHelper.cs") ||
            path.EndsWith("QQQ.cs") ||
            path.EndsWith("GamedevToolbelt.cs") ||
            path.EndsWith("CodeTODOsPrefs.cs") ||
            path.EndsWith("ScriptsPostProcessor.cs"))
        {
            return currentQQQs;
        }

        var lines = File.ReadAllLines(path);

        QQQ newQQQ;
        for (int i = 0; i < lines.Length; i++)
        {
            newQQQ = new QQQ();
            if (lines[i].Contains(CodeTODOsPrefs.TODOToken))
            {
                var index = lines[i].IndexOf(CodeTODOsPrefs.TODOToken);
                var hasExplicitPriority = false;

                // First we find the QQQ's priority.
                // QQQ1 means urgent, QQQ2 means normal, QQQ3 means minor. In case there's nothing (or something else/incorrect), we default to normal.
                switch (lines[i][index + 3])
                {
                    case '1':
                        newQQQ.Priority = QQQPriority.URGENT;
                        hasExplicitPriority = true;
                        break;
                    case '2':
                        newQQQ.Priority = QQQPriority.NORMAL;
                        hasExplicitPriority = true;
                        break;
                    case '3':
                        newQQQ.Priority = QQQPriority.MINOR;
                        hasExplicitPriority = true;
                        break;
                    default:
                        newQQQ.Priority = QQQPriority.NORMAL;
                        break;
                }

                // After the priority we get the task.
                // If the QQQ has an explicit priority, we add 1 to the index so that the number doesn't appear in the task.
                if (hasExplicitPriority == true)
                {
                    index += 1;
                }
                var tempString = lines[i].Substring(index);
                tempString = tempString.Substring(CodeTODOsPrefs.TODOToken.Length);
                tempString.Trim();
                newQQQ.Task = tempString;

                // Third, we save the source script.
                newQQQ.Script = path;

                currentQQQs.Add(newQQQ);
            }
        }
        return currentQQQs;
    }

    public static void AddQQQs(string script)
    {
        var qqqs = CodeTODOsHelper.GetQQQsFromScript(script);

        for (int i = 0; i < qqqs.Count; i++)
        {
            if (!CodeTODOs.QQQs.Contains(qqqs[i]))
            {
                //UnityEngine.Debug.Log("Added QQQ");
                CodeTODOs.QQQs.Add(qqqs[i]);
            }
        }
    }

    public static void RemoveScript(string script)
    {
        for(int i = 0; i < CodeTODOs.QQQs.Count; i++)
        {
            if(CodeTODOs.QQQs[i].Script == script)
            {
                CodeTODOs.QQQs.Remove(CodeTODOs.QQQs[i]);
                i--;
                //UnityEngine.Debug.Log("Removed QQQ");
            }
        }
    }

    public static void ChangeScriptOfQQQ(string fromPath, string toPath)
    {
        for (int i = 0; i < CodeTODOs.QQQs.Count; i++)
        {
            if (CodeTODOs.QQQs[i].Script == fromPath)
            {
                CodeTODOs.QQQs[i].Script = toPath;
                //UnityEngine.Debug.Log("Moved QQQ");
            }
        }
    }

    public static string GetStringEnd (string completeString, int numberOfCharacters)
    {
        if(numberOfCharacters >= completeString.Length)
        {
            return completeString;
        }
        var startIndex = completeString.Length - numberOfCharacters;
        return completeString.Substring(startIndex);
    }

    public static string DivideStringWithNewlines(string completeString, int numberOfCharacters)
    {
        if(numberOfCharacters >= completeString.Length)
        {
            return completeString;
        }

        int newLines = completeString.Length / numberOfCharacters;
        var index = numberOfCharacters;
        string newString = completeString;
        for (int i = 0; i < newLines; i++)
        {
            var subStr1 = newString.Substring(0, index);
            var subStr2 = newString.Substring(index);

            newString = subStr1 + "\n" + subStr2;
            index += 2 + numberOfCharacters;
        }
        return newString;
    }

    // Reorder the given QQQ list based on the urgency of tasks.
    public static void ReorderQQQs()
    {
        var originalQQQs = CodeTODOs.QQQs;
        var orderedQQQs = new List<QQQ>();

        // First add urgent tasks.
        for(int i = 0; i < originalQQQs.Count; i++)
        {
            if(originalQQQs[i].Priority == QQQPriority.URGENT)
            {
                orderedQQQs.Add(originalQQQs[i]);
            }
        }

        // Then normal ones.
        for(int i = 0; i < originalQQQs.Count; i++)
        {
            if(originalQQQs[i].Priority == QQQPriority.NORMAL)
            {
                orderedQQQs.Add(originalQQQs[i]);
            }
        }

        // Then minor ones.
        for(int i = 0; i < originalQQQs.Count; i++)
        {
            if(originalQQQs[i].Priority == QQQPriority.MINOR)
            {
                orderedQQQs.Add(originalQQQs[i]);
            }
        }

        CodeTODOs.QQQs = orderedQQQs;
    }
}