using UnityEngine;

public class BTLog : BTBaseNode
{
    protected string textToLog;

    public BTLog(string textToLog)
    {
        this.textToLog = textToLog;
    }

    protected override TaskStatus OnUpdate()
    {
        Debug.Log(textToLog);
        return TaskStatus.Success;
    }
}
