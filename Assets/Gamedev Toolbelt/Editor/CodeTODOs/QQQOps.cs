﻿using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace com.immortalhydra.gdtb.codetodos
{
    public static class QQQOps
    {

#region FIELDS AND PROPERTIES

        public static List<string> AllScripts = new List<string>();

#endregion


#region METHODS
        /// Find all files ending with .cs or .js (exclude those in exclude.txt).
        public static void FindAllScripts()
        {
            var assetsPaths = AssetDatabase.GetAllAssetPaths();

            var excludedScripts = IO.GetExcludedScripts();
            AllScripts = new List<string>();
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
                    AllScripts.Add(path);
                }
            }

            IO.SaveScriptList();
        }


        /// Find all QQQs in all scripts.
        public static void GetQQQsFromAllScripts()
        {
            var qqqs = new List<QQQ>();

            foreach (var script in AllScripts)
            {
                qqqs.AddRange(GetQQQsFromScript(script));
            }
            WindowMain.QQQs = qqqs;
        }


        /// Find the QQQs in a single script.
        public static List<QQQ> GetQQQsFromScript(string aPath)
        {
            var currentQQQs = new List<QQQ>();
            var lines = File.ReadAllLines(aPath);

            for (var i = 0; i < lines.Length; i++)
            {
                var newQQQ = new QQQ();
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
                    if (hasExplicitPriority)
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
            var qqqs = GetQQQsFromScript(aScript);

            foreach (var qqq in qqqs)
            {
                if (!WindowMain.QQQs.Contains(qqq))
                {
                    WindowMain.QQQs.Add(qqq);
                }
            }
        }


        /// Remove all references to the given script in CodeTODOs.QQQs.
        public static void RemoveScript(string aScript)
        {
            // Remove from QQQs.
            for (var i = 0; i < WindowMain.QQQs.Count; i++)
            {
                if (WindowMain.QQQs[i].Script == aScript)
                {
                    WindowMain.QQQs.Remove(WindowMain.QQQs[i]);
                    i--;
                }
            }

            // Remove from AllScripts
            for (var i = 0; i < AllScripts.Count; i++)
            {
                if (AllScripts[i] == aScript)
                {
                    AllScripts.Remove(AllScripts[i]);
                    break; // We have just one instance, we can exit the loop.
                }
            }
        }


        /// Change all references to a script in CodeTODOs.QQQs to another script (for when a script is moved).
        public static void ChangeScriptOfQQQ(string aPathTo, string aPathFrom)
        {
            foreach (var qqq in WindowMain.QQQs)
            {
                if (qqq.Script == aPathTo)
                {
                    qqq.Script = aPathFrom;
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
            foreach (var originalQQQ in originalQQQs)
            {
                if (originalQQQ.Priority == QQQPriority.URGENT)
                {
                    orderedQQQs.Add(originalQQQ);
                }
            }

            // Then normal ones.
            foreach (var originalQQQ in originalQQQs)
            {
                if (originalQQQ.Priority == QQQPriority.NORMAL)
                {
                    orderedQQQs.Add(originalQQQ);
                }
            }

            // Then minor ones.
            foreach (var originalQQQ in originalQQQs)
            {
                if (originalQQQ.Priority == QQQPriority.MINOR)
                {
                    orderedQQQs.Add(originalQQQ);
                }
            }

            WindowMain.QQQs = orderedQQQs;
        }


        /// Remove a QQQ (both from the list and from the file in which it was written).
        public static void CompleteQQQ(QQQ aQQQ)
        {
            IO.RemoveLineFromFile(aQQQ.Script, aQQQ.LineNumber);
            RefreshQQQs();
        }


        /// Remove a QQQ (both from the list and from the file in which it was written).
        public static void RemoveQQQ(QQQ aQQQ) // Different from the method above because we don't want removing to be the same as completing (for future integrations in which the concepts are different).
        {
            IO.RemoveLineFromFile(aQQQ.Script, aQQQ.LineNumber);
            RefreshQQQs();
        }


        /// Remove all QQQs.
        public static void RemoveAllQQQs()
        {
            foreach (var qqq in WindowMain.QQQs)
            {
                RemoveQQQ(qqq);
            }
            RefreshQQQs();
        }


        /// Open the script associated with the qqq in question.
        public static void OpenScript(QQQ aQQQ)
        {
        #if UNITY_5_3_OR_NEWER
            var script = AssetDatabase.LoadAssetAtPath<TextAsset>(aQQQ.Script);
            AssetDatabase.OpenAsset(script.GetInstanceID(), aQQQ.LineNumber + 1);

        #elif UNITY_5
            var script = AssetDatabase.LoadAssetAtPath(aQQQ.Script, typeof(UnityEngine.TextAsset)) as UnityEngine.TextAsset;
            AssetDatabase.OpenAsset(script.GetInstanceID(), (aQQQ.LineNumber + 1));

        #elif UNITY_4
            var script = AssetDatabase.LoadAssetAtPath<UnityEngine.TextAsset>(aQQQ.Script, typeof(UnityEngine.TextAsset)) as UnityEngine.TextAsset;
            AssetDatabase.OpenAsset(script.GetInstanceID(), (aQQQ.LineNumber + 1));

        #endif
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
        public static void RefreshQQQs()
        {
            WindowMain.QQQs.Clear();
            GetQQQsFromAllScripts();
            ReorderQQQs();
        }


        /// Create a new QQQ at the beginning of a script.
        public static void AddQQQ(QQQ aQQQ)
        {
            IO.AddQQQ(aQQQ);
            RefreshQQQs();
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

#endregion

    }
}