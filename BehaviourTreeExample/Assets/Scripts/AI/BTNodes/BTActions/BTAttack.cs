public class BTAttack : BTBaseNode
{
    protected override TaskStatus OnUpdate()
    {
        blackboard.SetVariable(VariableNames.STATE, State.ATTACKING);
        // DEAL DAMAGE
        return TaskStatus.Success;
    }
}
