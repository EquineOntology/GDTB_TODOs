#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CodeTODOsHelper
{
    ///2 Find all files ending with .cs or .js.
    public static List<string> FindAllScripts()
    {
        var allAssets = AssetDatabase.GetAllAssetPaths();
        var allScripts = new List<string>();

        foreach (var path in allAssets)
        {
            // Whatever the token, we don't want to include these files in the
            // a bunch of false positives in these files, so we exclude them.
            if(path.EndsWith("CodeTODOs.cs") ||
                path.EndsWith("CodeTODOsHelper.cs") ||
                path.EndsWith("CodeTODOsPrefs.cs") ||
                path.EndsWith("CodeTODOsEdit.cs") ||
                path.EndsWith("CodeTODOsIO.cs") ||
                path.EndsWith("QQQ.cs") ||
                path.EndsWith("QQQPriority.cs") ||
                path.EndsWith("GUIConstants.cs") ||
                path.EndsWith("ScriptsPostProcessor.cs"))
            {
                continue;
            }

            if (path.EndsWith(".cs") || path.EndsWith(".js"))
            {
                allScripts.Add(path);
            }
        }
        return allScripts;
    }


    /// Find all QQQs in all scripts.
    public static void GetQQQsFromAllScripts()
    {
        var allScripts = FindAllScripts();
        var qqqs = new List<QQQ>();

        for (int i = 0; i < allScripts.Count; i++)
        {
                qqqs.AddRange(GetQQQsFromScript(allScripts[i]));
        }
        CodeTODOs.QQQs = qqqs;
    }


    /// Find the QQQs in a single script.
    public static List<QQQ> GetQQQsFromScript(string aPath)
    {
        var currentQQQs = new List<QQQ>();

        var lines = File.ReadAllLines(aPath);

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
                newQQQ.Script = aPath;

                // Lastly, we save the line number.
                newQQQ.LineNumber = i;

                currentQQQs.Add(newQQQ);
            }
        }
        return currentQQQs;
    }


    /// Add the QQQs in a script to the list in CodeTODOs.
    public static void AddQQQs(string aScript)
    {
        var qqqs = CodeTODOsHelper.GetQQQsFromScript(aScript);

        for (int i = 0; i < qqqs.Count; i++)
        {
            if (!CodeTODOs.QQQs.Contains(qqqs[i]))
            {
                //Debug.Log("Added QQQ");
                CodeTODOs.QQQs.Add(qqqs[i]);
            }
        }
    }


    /// Remove all references to the given script in CodeTODOs.QQQs.
    public static void RemoveScript(string aScript)
    {
        for (int i = 0; i < CodeTODOs.QQQs.Count; i++)
        {
            if (CodeTODOs.QQQs[i].Script == aScript)
            {
                CodeTODOs.QQQs.Remove(CodeTODOs.QQQs[i]);
                i--;
                //Debug.Log("Removed QQQ");
            }
        }
    }


    /// Change all references to a script in CodeTODOs.QQQs to another script (for when a script is moved).
    public static void ChangeScriptOfQQQ(string aPathTo, string aPathFrom)
    {
        for (int i = 0; i < CodeTODOs.QQQs.Count; i++)
        {
            if (CodeTODOs.QQQs[i].Script == aPathTo)
            {
                CodeTODOs.QQQs[i].Script = aPathFrom;
                //Debug.Log("Moved QQQ");
            }
        }
    }


    /// Get the last characters of a string.
    public static string GetStringEnd(string aCompleteString, int aNumberOfCharacters)
    {
        if (aNumberOfCharacters >= aCompleteString.Length)
        {
            return aCompleteString;
        }
        var startIndex = aCompleteString.Length - aNumberOfCharacters;
        return aCompleteString.Substring(startIndex);
    }


    /// Reorder the given QQQ list based on the urgency of tasks.
    public static void ReorderQQQs()
    {
        var originalQQQs = CodeTODOs.QQQs;
        var orderedQQQs = new List<QQQ>();

        // First add urgent tasks.
        for (int i = 0; i < originalQQQs.Count; i++)
        {
            if (originalQQQs[i].Priority == QQQPriority.URGENT)
            {
                orderedQQQs.Add(originalQQQs[i]);
            }
        }

        // Then normal ones.
        for (int i = 0; i < originalQQQs.Count; i++)
        {
            if (originalQQQs[i].Priority == QQQPriority.NORMAL)
            {
                orderedQQQs.Add(originalQQQs[i]);
            }
        }

        // Then minor ones.
        for (int i = 0; i < originalQQQs.Count; i++)
        {
            if (originalQQQs[i].Priority == QQQPriority.MINOR)
            {
                orderedQQQs.Add(originalQQQs[i]);
            }
        }

        CodeTODOs.QQQs = orderedQQQs;
    }


    /// Format a QQQ's script.
    public static string FormatScriptLabel(QQQ aQQQ, float aWidth, GUIStyle aStyle)
    {
        var scriptContent = new GUIContent(aQQQ.Script);
        var scriptWidth = aStyle.CalcSize(scriptContent).x;

        var additionalCharactersWidth = aStyle.CalcSize(new GUIContent("Line " + (aQQQ.LineNumber + 1).ToString() + " in \"\"")).x;
        var totalScriptWidth = scriptWidth + additionalCharactersWidth;

        var formattedScript = aQQQ.Script;
        if (scriptWidth >= aWidth)
        {
            formattedScript = CutStringAtWidth(aQQQ.Script, (int)aWidth, aStyle);
            formattedScript = "Line " + (aQQQ.LineNumber + 1) + " in \"" + formattedScript + "\"";
        }
        else
        {
            formattedScript = "Line " + (aQQQ.LineNumber + 1) + " in \"" + formattedScript + "\"";
        }

        return formattedScript;
    }


    /// Cut the length of a string and insert "..." at its beginning.
    private static string CutStringAtWidth(string aString, int aWidth, GUIStyle aStyle)
    {
        var stringWidth = aStyle.CalcSize(new GUIContent(aString)).x;
        var surplusWidth = aWidth - stringWidth;
        var surplusCharacters = surplusWidth / GUIConstants.NORMAL_CHAR_WIDTH;

        int cutoffIndex = (int)(Mathf.Clamp(surplusCharacters + 3, 0, aString.Length - 1)); // +3 because of the "..." we'll be adding.
        //Debug.Log("String: " + aString + " , Cutoff: " + cutoffIndex);
        return "..." + aString.Substring(cutoffIndex);
    }


    /// Remove a QQQ (both from the list and from the file in which it was written).
    public static void CompleteQQQ(QQQ aQQQ)
    {
        CodeTODOsIO.RemoveLineFromFile(aQQQ.Script, aQQQ.LineNumber);
        CodeTODOs.QQQs.Remove(aQQQ);
    }


    /// Open the script associated with the qqq in question.
    public static void OpenScript(QQQ aQQQ)
    {
        var script = AssetDatabase.LoadAssetAtPath<UnityEngine.TextAsset>(aQQQ.Script) as UnityEngine.TextAsset;
        AssetDatabase.OpenAsset(script.GetInstanceID(), (aQQQ.LineNumber + 1));
    }


    public static void UpdateTask(QQQ anOldQQQ, QQQ aNewQQQ)
    {
        CodeTODOsIO.ChangeQQQ(anOldQQQ, aNewQQQ);
    }
}
#endif