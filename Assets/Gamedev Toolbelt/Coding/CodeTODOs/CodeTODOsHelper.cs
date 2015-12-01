#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public static class CodeTODOsHelper
{

    // Find all files ending with .cs or .js.
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


    // Find all QQQs in all scripts.
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


    // Find the QQQs in a single script.
    public static List<QQQ> GetQQQsFromScript(string aPath)
    {
        var currentQQQs = new List<QQQ>();

        // Since the string "QQQ" is repeated many times in the files listed, its default value would give
        // a bunch of false positives in these files, so we exclude them.
        if (aPath.EndsWith("CodeTODOs.cs") ||
            aPath.EndsWith("CodeTODOsHelper.cs") ||
            aPath.EndsWith("QQQ.cs") ||
            aPath.EndsWith("GamedevToolbelt.cs") ||
            aPath.EndsWith("CodeTODOsPrefs.cs") ||
            aPath.EndsWith("CodeTODOsEdit.cs") ||
            aPath.EndsWith("QQQPriority.cs") ||
            aPath.EndsWith("CodeTODOsIO.cs") ||
            aPath.EndsWith("GUIConstants.cs") ||
            aPath.EndsWith("ScriptsPostProcessor.cs"))
        {
            return currentQQQs;
        }

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


    // Add the QQQs in a script to the list in CodeTODOs.
    public static void AddQQQs(string aScript)
    {
        var qqqs = CodeTODOsHelper.GetQQQsFromScript(aScript);

        for (int i = 0; i < qqqs.Count; i++)
        {
            if (!CodeTODOs.QQQs.Contains(qqqs[i]))
            {
                //UnityEngine.Debug.Log("Added QQQ");
                CodeTODOs.QQQs.Add(qqqs[i]);
            }
        }
    }


    // Remove all references to the given script in CodeTODOs.QQQs.
    public static void RemoveScript(string aScript)
    {
        for (int i = 0; i < CodeTODOs.QQQs.Count; i++)
        {
            if (CodeTODOs.QQQs[i].Script == aScript)
            {
                CodeTODOs.QQQs.Remove(CodeTODOs.QQQs[i]);
                i--;
                //UnityEngine.Debug.Log("Removed QQQ");
            }
        }
    }


    // Change all references to a script in CodeTODOs.QQQs to another script (for when a script is moved).
    public static void ChangeScriptOfQQQ(string aPathTo, string aPathFrom)
    {
        for (int i = 0; i < CodeTODOs.QQQs.Count; i++)
        {
            if (CodeTODOs.QQQs[i].Script == aPathTo)
            {
                CodeTODOs.QQQs[i].Script = aPathFrom;
                //UnityEngine.Debug.Log("Moved QQQ");
            }
        }
    }


    // Get the last characters of a string.
    public static string GetStringEnd(string aCompleteString, int aNumberOfCharacters)
    {
        if (aNumberOfCharacters >= aCompleteString.Length)
        {
            return aCompleteString;
        }
        var startIndex = aCompleteString.Length - aNumberOfCharacters;
        return aCompleteString.Substring(startIndex);
    }





    // Reorder the given QQQ list based on the urgency of tasks.
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


    // Formats a label (a qqq script or task) based on preferences.
    public static string[] FormatTaskAndScriptLabels(QQQ aQQQ, float aWidth)
    {
        var formattedLabels = new string[2];

        var rectWidth = aWidth;
        var taskWidth = GetWidthOfString(aQQQ.Task, true);

        // If the task is larger than the rect, we divide it in newlines.
        if (taskWidth >= rectWidth)
        {
            var formattedTask = DivideStringWithNewlines(aQQQ.Task, (int)(rectWidth / GUIConstants.BOLD_CHAR_WIDTH));
            formattedLabels[0] = formattedTask;
        }
        else
        {
            formattedLabels[0] = aQQQ.Task;
        }

        // Calculate the width of the whole script.
        var scriptWidth = GetWidthOfString(aQQQ.Script);
        var additionalCharactersWidth = ("Line ".Length + (aQQQ.LineNumber + 1).ToString().Length + " in \"\"".Length) * GUIConstants.NORMAL_CHAR_WIDTH;
        var totalScriptWidth = scriptWidth + additionalCharactersWidth;

        var formattedScript = aQQQ.Script;
        if (totalScriptWidth >= rectWidth)
        {
            formattedScript = CutStringAtWidth(aQQQ.Script, (int)aWidth);
        }
        formattedLabels[1] = "Line " + (aQQQ.LineNumber + 1) + " in \"" + formattedScript + "\"";

        return formattedLabels;
    }


    // Remove a QQQ (both from the list and from the file in which it was written).
    public static void CompleteQQQ(QQQ aQQQ)
    {
        CodeTODOsIO.RemoveLineFromFile(aQQQ.Script, aQQQ.LineNumber);
        CodeTODOs.QQQs.Remove(aQQQ);
    }


    // Open the script associated with the qqq in question.
    public static void OpenScript(QQQ aQQQ)
    {
        var script = AssetDatabase.LoadAssetAtPath<UnityEngine.TextAsset>(aQQQ.Script) as UnityEngine.TextAsset;
        AssetDatabase.OpenAsset(script.GetInstanceID(), (aQQQ.LineNumber + 1));
    }


    public static void UpdateTask(QQQ anOldQQQ, QQQ aNewQQQ)
    {
        CodeTODOsIO.ChangeQQQ(anOldQQQ, aNewQQQ);
    }


    // Return the width of a string based on its length and style.
    private static float GetWidthOfString(string aString, bool isBold = false)
    {
        if (isBold)
        {
            return aString.Length * GUIConstants.BOLD_CHAR_WIDTH;
        }
        return aString.Length * GUIConstants.NORMAL_CHAR_WIDTH;
    }


    // Insert \n (newline characters) in a string, based on the limit provided.
    public static string DivideStringWithNewlines(string aCompleteString, int aNumberOfCharacters)
    {
        if (aNumberOfCharacters >= aCompleteString.Length)
        {
            return aCompleteString;
        }

        var newLines = aNumberOfCharacters != 0 ? aCompleteString.Length / aNumberOfCharacters : aCompleteString.Length;
        var index = aNumberOfCharacters;
        var newString = aCompleteString;
        for (int i = 0; i < newLines; i++)
        {
            var subStr1 = newString.Substring(0, index);
            var subStr2 = newString.Substring(index);

            newString = subStr1 + "\n" + subStr2;

            index += 1 + aNumberOfCharacters; // \n counts as a single character.
        }
        return newString;
    }


    // Cut the length of a string and insert "..." at its beginning.
    private static string CutStringAtWidth(string aString, int aMaxWidth, bool isBold = false)
    {
        var stringWidth = aString.Length * (isBold ? GUIConstants.BOLD_CHAR_WIDTH : GUIConstants.NORMAL_CHAR_WIDTH);
        var surplusWidth = aMaxWidth - stringWidth;
        var surplusCharacters = surplusWidth / (isBold ? GUIConstants.BOLD_CHAR_WIDTH : GUIConstants.NORMAL_CHAR_WIDTH);

        int cutoffIndex = UnityEngine.Mathf.Clamp(surplusCharacters + 3, 0, aString.Length - 1); // +3 because of the "..." we'll be adding.
        //UnityEngine.Debug.Log("String: " + aString + " , Cutoff: " + cutoffIndex);
        return "..." + aString.Substring(cutoffIndex);
    }

    public static float CalculateHeightOfString(string aString, float aMaxWidth, bool isBold = false)
    {
        //UnityEngine.Debug.Log("string: " + aString);
        var stringWidth = aString.Length * (isBold ? GUIConstants.BOLD_CHAR_WIDTH : GUIConstants.NORMAL_CHAR_WIDTH);
        //UnityEngine.Debug.Log("width: " + stringWidth);
        var lines = UnityEngine.Mathf.Clamp((int)(stringWidth / aMaxWidth), 1, 300);
        //UnityEngine.Debug.Log("lines: " + lines);
        var height =  lines * (GUIConstants.CHAR_HEIGHT * GUIConstants.VERTICAL_SPACING); // Times 1.2 to take line spacing into account.
        //UnityEngine.Debug.Log("height: " + height);
        return height;
    }
}
#endif