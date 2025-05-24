public class BTAttack : BTBaseNode
{
    protected override TaskStatus OnUpdate()
    {
        blackboard.SetVariable(VariableNames.PLAYER_DEAD, true);
        blackboard.SetVariable(VariableNames.SEES_PLAYER, false);
        return TaskStatus.Success;
    }
}
