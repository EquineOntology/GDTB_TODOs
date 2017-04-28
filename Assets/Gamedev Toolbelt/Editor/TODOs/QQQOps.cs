using System;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace com.immortalhydra.gdtb.todos
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
                    var index = lines[i].IndexOf(Preferences.TODOToken, StringComparison.Ordinal);
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


        /// Add the QQQs in a script to the list in TODOs.
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


        /// Remove all references to the given script in TODOs.QQQs.
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


        /// Change all references to a script in TODOs.QQQs to another script (for when a script is moved).
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

            // First are pinned tasks (ordered urgent-normal-minor), then urgent, then normal, then minor ones.
            var pinnedQQQsUrgent = new List<QQQ>();
            var pinnedQQQsNormal = new List<QQQ>();
            var pinnedQQQsMinor = new List<QQQ>();
            var urgentQQQs = new List<QQQ>();
            var normalQQQs = new List<QQQ>();
            var minorQQQs = new List<QQQ>();
            foreach (var qqq in originalQQQs)
            {
                if (qqq.IsPinned)
                {
                    switch (qqq.Priority)
                    {
                        case QQQPriority.URGENT:
                            pinnedQQQsUrgent.Add(qqq);
                            break;
                        case QQQPriority.NORMAL:
                            pinnedQQQsNormal.Add(qqq);
                            break;
                        default:
                            pinnedQQQsMinor.Add(qqq);
                            break;
                    }
                }
                else
                {
                    switch (qqq.Priority)
                    {
                        case QQQPriority.URGENT:
                            urgentQQQs.Add(qqq);
                            break;
                        case QQQPriority.NORMAL:
                            normalQQQs.Add(qqq);
                            break;
                        default:
                            minorQQQs.Add(qqq);
                            break;
                    }
                }
            }
            orderedQQQs.AddRange(pinnedQQQsUrgent);
            orderedQQQs.AddRange(pinnedQQQsNormal);
            orderedQQQs.AddRange(pinnedQQQsMinor);
            orderedQQQs.AddRange(urgentQQQs);
            orderedQQQs.AddRange(normalQQQs);
            orderedQQQs.AddRange(minorQQQs);

            WindowMain.QQQs = orderedQQQs;
        }


        /// Remove a QQQ (both from the list and from the file in which it was written).
        public static void RemoveQQQFromList(QQQ aQQQ)
        {
            WindowMain.CompletedQQQs.Add(aQQQ);
            WindowMain.QQQs.Remove(aQQQ);
            WindowMain.QQQsChanged = true;
        }


        /// Remove all QQQs.
        public static void RemoveAllQQQs()
        {
            foreach (var qqq in WindowMain.QQQs)
            {
                RemoveQQQFromList(qqq);
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
            var pinnedQQQs = WindowMain.QQQs.Where(qqq => qqq.IsPinned).ToList();

            WindowMain.QQQs.Clear();
            GetQQQsFromAllScripts();

            foreach (var qqq in pinnedQQQs)
            {
                foreach (var currentQQQ in WindowMain.QQQs)
                {
                    if (qqq.LineNumber == currentQQQ.LineNumber &&
                        qqq.Script == currentQQQ.Script &&
                        qqq.Task == currentQQQ.Task &&
                        qqq.Priority == currentQQQ.Priority)
                    {
                        currentQQQ.IsPinned = true;
                        break;
                    }
                }
            }

            pinnedQQQs.Clear();
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