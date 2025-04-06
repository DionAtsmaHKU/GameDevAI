public class BTSetState : BTBaseNode
{
    private State newState;
    public BTSetState(State state)
    {
        newState = state;
    }

    protected override TaskStatus OnUpdate()
    {
        blackboard.SetVariable(VariableNames.STATE, newState);
        return TaskStatus.Success;
    }
}
