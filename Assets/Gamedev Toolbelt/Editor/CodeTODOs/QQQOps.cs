using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GDTB.CodeTODOs
{
    public static class QQQOps
    {
        /// Find all files ending with .cs or .js (exclude those in exclude.txt).
        public static List<string> FindAllScripts()
        {
            var assetsPaths = AssetDatabase.GetAllAssetPaths();

            var excludedScripts = IO.GetExcludedScripts();
            var allScripts = new List<string>();
            foreach (var path in assetsPaths)
            {
                // There are some files we don't want to include.
                var shouldBeExcluded = false;
                if (path.EndsWith(".cs") || path.EndsWith(".js"))
                {
                    foreach (var exclusion in excludedScripts)
                    {
                        if (path.Contains(exclusion)) // This works for both files and directories.
                        {
                            shouldBeExcluded = true;
                        }
                    }
                }
                else
                {
                    shouldBeExcluded = true;
                }

                if (shouldBeExcluded == false)
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
            WindowMain.QQQs = qqqs;
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
                if (lines[i].Contains(Preferences.TODOToken))
                {
                    var index = lines[i].IndexOf(Preferences.TODOToken);
                    var hasExplicitPriority = false;

                    // First we find the QQQ's priority.
                    // QQQ1 means urgent, QQQ2 means normal, QQQ3 means minor. In case there's nothing (or something else/incorrect), we default to normal.
                    switch (lines[i][index + Preferences.TODOToken.Length])
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
                    tempString = tempString.Substring(Preferences.TODOToken.Length);
                    tempString = tempString.Trim();
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
            var qqqs = QQQOps.GetQQQsFromScript(aScript);

            for (int i = 0; i < qqqs.Count; i++)
            {
                if (!WindowMain.QQQs.Contains(qqqs[i]))
                {
                    WindowMain.QQQs.Add(qqqs[i]);
                }
            }
        }


        /// Remove all references to the given script in CodeTODOs.QQQs.
        public static void RemoveScript(string aScript)
        {
            for (int i = 0; i < WindowMain.QQQs.Count; i++)
            {
                if (WindowMain.QQQs[i].Script == aScript)
                {
                    WindowMain.QQQs.Remove(WindowMain.QQQs[i]);
                    i--;
                }
            }
        }


        /// Change all references to a script in CodeTODOs.QQQs to another script (for when a script is moved).
        public static void ChangeScriptOfQQQ(string aPathTo, string aPathFrom)
        {
            for (int i = 0; i < WindowMain.QQQs.Count; i++)
            {
                if (WindowMain.QQQs[i].Script == aPathTo)
                {
                    WindowMain.QQQs[i].Script = aPathFrom;
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
            var originalQQQs = WindowMain.QQQs;
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

            WindowMain.QQQs = orderedQQQs;
        }


        /// Format a QQQ's script.
        public static string CreateScriptLabel(QQQ aQQQ, float aWidth, GUIStyle aStyle)
        {
            var scriptContent = new GUIContent(aQQQ.Script);
            var scriptWidth = aStyle.CalcSize(scriptContent).x;

            var additionalCharactersWidth = aStyle.CalcSize(new GUIContent("Line " + (aQQQ.LineNumber + 1).ToString() + " in \"\"")).x;

            var formattedScript = aQQQ.Script;
            var permittedWidth = aWidth - additionalCharactersWidth;
            if (scriptWidth > permittedWidth)
            {
                formattedScript = ReduceScriptPath(aQQQ, permittedWidth, aStyle);
                formattedScript = "Line " + (aQQQ.LineNumber + 1) + " in \"" + formattedScript + "\"";
            }
            else
            {
                formattedScript = "Line " + (aQQQ.LineNumber + 1) + " in \"" + formattedScript + "\"";
            }

            return formattedScript;
        }


        /// If the script path is wider than its rect, cut it and insert "..."
        private static string ReduceScriptPath(QQQ aQQQ, float aWidth, GUIStyle aStyle)
        {
            var stringWidth = aStyle.CalcSize(new GUIContent(aQQQ.Script)).x;
            var surplusWidth = stringWidth - aWidth;
            var surplusCharacters = (int)Mathf.Ceil(surplusWidth / Constants.NORMAL_CHAR_WIDTH);

            int cutoffIndex = Mathf.Clamp(surplusCharacters + 4, 0, aQQQ.Script.Length - 1); // +4 because of the "..." we'll be adding.
            return "..." + aQQQ.Script.Substring(cutoffIndex);
        }


        /// Remove a QQQ (both from the list and from the file in which it was written).
        public static void CompleteQQQ(QQQ aQQQ)
        {
            IO.RemoveLineFromFile(aQQQ.Script, aQQQ.LineNumber);
            RefreshList();

        }


        /// Open the script associated with the qqq in question.
        public static void OpenScript(QQQ aQQQ)
        {
            var script = AssetDatabase.LoadAssetAtPath<UnityEngine.TextAsset>(aQQQ.Script) as UnityEngine.TextAsset;
            AssetDatabase.OpenAsset(script.GetInstanceID(), (aQQQ.LineNumber + 1));
        }


        /// Change the task of a QQQ.
        public static void UpdateTask(QQQ anOldQQQ, QQQ aNewQQQ)
        {
            IO.ChangeQQQ(anOldQQQ, aNewQQQ);
            for (var i = 0; i < WindowMain.QQQs.Count; i++)
            {
                if (WindowMain.QQQs[i].Script == aNewQQQ.Script && WindowMain.QQQs[i].LineNumber == aNewQQQ.LineNumber)
                {
                    WindowMain.QQQs[i] = aNewQQQ;
                    break;
                }
            }
        }


        /// Re-import all QQQs.
        public static void RefreshList()
        {
            WindowMain.QQQs.Clear();
            QQQOps.GetQQQsFromAllScripts();
            QQQOps.ReorderQQQs();
        }


        /// Create a new QQQ at the beginning of a script.
        public static void AddQQQ(QQQ aQQQ)
        {
            IO.AddQQQ(aQQQ);
            //var adjustedQQQ = aQQQ;
            //adjustedQQQ.LineNumber -= 1;
            //WindowMain.QQQs.Add(adjustedQQQ);
            RefreshList();
            EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
        }


        /// Get the int equivalent of a QQQPriority.
        public static int PriorityToInt(QQQPriority aPriority)
        {
            switch (aPriority)
            {
                case QQQPriority.URGENT:
                    return 1;
                case QQQPriority.MINOR:
                    return 3;
                default:
                    return 2;
            }
        }


        /// Get the QQQPriority equivalent of an int
        public static QQQPriority IntToPriority(int anInt)
        {
            switch (anInt)
            {
                case 1:
                    return QQQPriority.URGENT;
                case 3:
                    return QQQPriority.MINOR;
                default:
                    return QQQPriority.NORMAL;
            }
        }
    }
}