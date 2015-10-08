using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class CodeTODOsHelper
{
    public static List<string> FindAllScripts()
    {
        var allAssets = AssetDatabase.GetAllAssetPaths();
        var allScripts = new List<string> ();

        foreach (var path in allAssets)
        {
            if (path.EndsWith(".cs") || path.EndsWith(".js"))
            {
                allScripts.Add(path);
            }
        }
        return allScripts;
    }

    public static List<QQQ> CheckAllScriptsForQQQs(out List<string> qqqTasks, out List<string> scripts)
    {
        // Since we're rechecking everything, let's clear the two lists.
        scripts = new List<string>();
        qqqTasks = new List<string>();

        // First we collect all scripts in the project, and then we check them for QQQs.
        var AllScripts = FindAllScripts();
        foreach(var script in AllScripts)
        {
            var QQQsInScript = CheckScriptForQQQs(script);

            for(int i = 0; i < QQQsInScript.Count; i++)
            {
                // Since the string "QQQ" is repeated many times in these three files listed, its default value would give
                // a bunch of false positives in these files. So either the token doesn't use the default value,
                // or we exclude these three files from the collection.
                if (!script.EndsWith("CodeTODOs.cs") &&
                    !script.EndsWith("CodeTODOsHelper.cs") &&
                    !script.EndsWith("QQQ.cs") &&
                    !script.EndsWith("GamedevToolbelt.cs"))
                {
                    scripts.Add(script);
                    qqqTasks.Add(QQQsInScript[i]);
                }
            }
        }

        var qqqs = new List<QQQ>();
        for (int j = 0; j < scripts.Count; j++)
        {
            qqqs.Add(new QQQ(qqqTasks[j], scripts[j]));
        }

        return qqqs;
    }

    private static List<string> CheckScriptForQQQs(string path)
    {
        var currentQQQs = new List<string>();

        var lines = File.ReadAllLines(path);
        string completeQQQ;
        for (int i = 0; i < lines.Length; i++)
        {
            completeQQQ = "";
            if (lines[i].Contains(CodeTODOs.TODOToken))
            {
                var index = lines[i].IndexOf(CodeTODOs.TODOToken);
                var tempString = lines[i].Substring(index);
                tempString = tempString.Substring(CodeTODOs.TODOToken.Length);
                tempString.Trim();
                completeQQQ += tempString;

                currentQQQs.Add(completeQQQ);
            }
        }
        return currentQQQs;
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
}