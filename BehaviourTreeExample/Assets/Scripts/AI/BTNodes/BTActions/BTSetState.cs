public class BTSetState : BTBaseNode
{
    private State newState;
    public BTSetState(State state)
    {
        newState = state;
    }

    protected override TaskStatus OnUpdate()
    {
        if (blackboard.GetVariable<State>(VariableNames.STATE) == newState)
            return TaskStatus.Success;
        
        blackboard.SetVariable(VariableNames.STATE, newState);
        return TaskStatus.Success;
    }
}
