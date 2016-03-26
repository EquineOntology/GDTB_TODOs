namespace GDTB.CodeTODOs
{
    [System.Serializable]
    public class QQQ: System.Object
    {
        public QQQPriority Priority;
        public string Task;
        public string Script;
        public int LineNumber;


        public QQQ(int aPriority, string aTask, string aScript, int aLineNumber)
        {
            switch(aPriority)
            {
                case 1:
                    this.Priority = QQQPriority.URGENT;
                    break;
                case 2:
                    this.Priority = QQQPriority.NORMAL;
                    break;
                case 3:
                    this.Priority = QQQPriority.MINOR;
                    break;
                default:
                    this.Priority = QQQPriority.NORMAL;
                    break;
            }
            this.Task = aTask;
            this.Script = aScript;
            this.LineNumber = aLineNumber;
        }


        public QQQ(QQQPriority aPriority, string aTask, string aScript, int aLineNumber)
        {
            this.Priority = aPriority;
            this.Task = aTask;
            this.Script = aScript;
            this.LineNumber = aLineNumber;
        }


        public QQQ(string aTask, string aScript)
        {
            this.Priority = QQQPriority.NORMAL;
            this.Task = aTask;
            this.Script = aScript;
            this.LineNumber = 0;
        }


        public QQQ()
        {
            this.Priority = QQQPriority.NORMAL;
            this.Task = "";
            this.Script = "";
            this.LineNumber = 0;
        }
    }
}