[System.Serializable]
public class QQQ: System.Object
{
    //public int Priority;
    public string Task;
    public string Script;

    public QQQ(/*int priority,*/ string task, string script)
    {
        //this.Priority = priority;
        this.Task = task;
        this.Script = script;
    }
}
