namespace com.immortalhydra.gdtb.todos
{
    [System.Serializable]
    public class QQQ: object
    {

#region FIELDS AND PROPERTIES

        public QQQPriority Priority;
        public string Task;
        public string Script;
        public int LineNumber;
        public bool IsPinned;

#endregion

#region CONSTRUCTORS

        public QQQ(int aPriority, string aTask, string aScript, int aLineNumber)
        {
            switch(aPriority)
            {
                case 1:
                    Priority = QQQPriority.URGENT;
                    break;
                case 2:
                    Priority = QQQPriority.NORMAL;
                    break;
                case 3:
                    Priority = QQQPriority.MINOR;
                    break;
                default:
                    Priority = QQQPriority.NORMAL;
                    break;
            }
            Task = aTask;
            Script = aScript;
            LineNumber = aLineNumber;
            IsPinned = false;
        }


        public QQQ(int aPriority, string aTask, string aScript, int aLineNumber, bool isPinned)
        {
            switch(aPriority)
            {
                case 1:
                    Priority = QQQPriority.URGENT;
                    break;
                case 2:
                    Priority = QQQPriority.NORMAL;
                    break;
                case 3:
                    Priority = QQQPriority.MINOR;
                    break;
                default:
                    Priority = QQQPriority.NORMAL;
                    break;
            }
            Task = aTask;
            Script = aScript;
            LineNumber = aLineNumber;
            IsPinned = isPinned;
        }


        public QQQ(QQQPriority aPriority, string aTask, string aScript, int aLineNumber)
        {
            Priority = aPriority;
            Task = aTask;
            Script = aScript;
            LineNumber = aLineNumber;
            IsPinned = false;
        }


        public QQQ(string aTask, string aScript)
        {
            Priority = QQQPriority.NORMAL;
            Task = aTask;
            Script = aScript;
            LineNumber = 0;
            IsPinned = false;
        }


        public QQQ()
        {
            Priority = QQQPriority.NORMAL;
            Task = "";
            Script = "";
            LineNumber = 0;
            IsPinned = false;
        }

#endregion

    }
}