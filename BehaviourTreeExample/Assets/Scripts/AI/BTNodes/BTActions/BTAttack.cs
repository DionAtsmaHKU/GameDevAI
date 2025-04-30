public class BTAttack : BTBaseNode
{
    protected override TaskStatus OnUpdate()
    {
        blackboard.SetVariable(VariableNames.STATE, State.ATTACKING);
        blackboard.SetVariable(VariableNames.PLAYER_DEAD, true);
        return TaskStatus.Success;
    }
}
