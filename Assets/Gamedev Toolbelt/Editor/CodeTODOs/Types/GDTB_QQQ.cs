[System.Serializable]
public class GDTB_QQQ: System.Object
{
    public GDTB_QQQPriority Priority;
    public string Task;
    public string Script;
    public int LineNumber;


    public GDTB_QQQ(int aPriority, string aTask, string aScript, int aLineNumber)
    {
        switch(aPriority)
        {
            case 1:
                this.Priority = GDTB_QQQPriority.URGENT;
                break;
            case 2:
                this.Priority = GDTB_QQQPriority.NORMAL;
                break;
            case 3:
                this.Priority = GDTB_QQQPriority.MINOR;
                break;
            default:
                this.Priority = GDTB_QQQPriority.NORMAL;
                break;
        }
        this.Task = aTask;
        this.Script = aScript;
        this.LineNumber = aLineNumber;
    }


    public GDTB_QQQ(GDTB_QQQPriority aPriority, string aTask, string aScript, int aLineNumber)
    {
        this.Priority = aPriority;
        this.Task = aTask;
        this.Script = aScript;
        this.LineNumber = aLineNumber;
    }


    public GDTB_QQQ(string aTask, string aScript)
    {
        this.Priority = GDTB_QQQPriority.NORMAL;
        this.Task = aTask;
        this.Script = aScript;
        this.LineNumber = 0;
    }


    public GDTB_QQQ()
    {
        this.Priority = GDTB_QQQPriority.NORMAL;
        this.Task = "";
        this.Script = "";
        this.LineNumber = 0;
    }
}