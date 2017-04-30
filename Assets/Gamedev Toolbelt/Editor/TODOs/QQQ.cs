using UnityEngine;

namespace com.immortalhydra.gdtb.todos
{
    [System.Serializable]
    public class QQQ : ScriptableObject
    {
        #region FIELDS AND PROPERTIES

        public QQQPriority Priority;
        public string Task;
        public string Script;
        public int LineNumber;
        public bool IsPinned;

        #endregion

        #region MONOBEHAVIOUR METHODS

        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        #endregion

        #region CONSTRUCTORS

        public static QQQ Create(int aPriority, string aTask, string aScript, int aLineNumber)
        {
            var qqq = CreateInstance<QQQ>();

            qqq.Priority = QQQOps.IntToPriority(aPriority);
            qqq.Task = aTask;
            qqq.Script = aScript;
            qqq.LineNumber = aLineNumber;
            qqq.IsPinned = false;

            return qqq;
        }


        public static QQQ Create(int aPriority, string aTask, string aScript, int aLineNumber, bool isPinned)
        {
            var qqq = CreateInstance<QQQ>();

            qqq.Priority = QQQOps.IntToPriority(aPriority);
            qqq.Task = aTask;
            qqq.Script = aScript;
            qqq.LineNumber = aLineNumber;
            qqq.IsPinned = isPinned;

            return qqq;
        }


        public static QQQ Create(QQQPriority aPriority, string aTask, string aScript, int aLineNumber)
        {
            var qqq = CreateInstance<QQQ>();

            qqq.Priority = aPriority;
            qqq.Task = aTask;
            qqq.Script = aScript;
            qqq.LineNumber = aLineNumber;
            qqq.IsPinned = false;

            return qqq;
        }


        public static QQQ Create(string aTask, string aScript)
        {
            var qqq = CreateInstance<QQQ>();

            qqq.Priority = QQQPriority.NORMAL;
            qqq.Task = aTask;
            qqq.Script = aScript;
            qqq.LineNumber = 0;
            qqq.IsPinned = false;

            return qqq;
        }

        public static QQQ Create()
        {
            var qqq = CreateInstance<QQQ>();
            qqq.Priority = QQQPriority.NORMAL;
            qqq.Task = "";
            qqq.Script = "";
            qqq.LineNumber = 0;
            qqq.IsPinned = false;

            return qqq;
        }

        #endregion
    }
}