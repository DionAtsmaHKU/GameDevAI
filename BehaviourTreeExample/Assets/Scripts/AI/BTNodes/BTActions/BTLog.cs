using TMPro;

// Logs a message to the UI above an agent.
public class BTLog : BTBaseNode
{
    protected string textToLog;
    private TextMeshProUGUI textUI;

    public BTLog(string text, TextMeshProUGUI ui)
    {
        textToLog = text;
        textUI = ui;
    }

    protected override TaskStatus OnUpdate()
    {
        textUI.text = "Current State: \n" + textToLog;
        return TaskStatus.Success;
    }
}
