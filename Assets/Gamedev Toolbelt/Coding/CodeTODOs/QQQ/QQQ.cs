[System.Serializable]
public class QQQ: System.Object
{
    public QQQPriority Priority;
    public string Task;
    public string Script;
    public int LineNumber;

    public QQQ(int priority, string task, string script, int lineNumber)
    {
        switch(priority)
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
        this.Task = task;
        this.Script = script;
        this.LineNumber = 0;
    }

    public QQQ(string task, string script)
    {
        this.Priority = QQQPriority.NORMAL;
        this.Task = task;
        this.Script = script;
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


