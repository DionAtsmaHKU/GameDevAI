public class BTStunEnemy : BTBaseNode
{
    private Guard guard;

    public BTStunEnemy(Guard guard)
    {
        this.guard = guard; 
    }

    protected override TaskStatus OnUpdate()
    {
        if (!guard.IsStunned())
            guard.Stun();

        return TaskStatus.Success;
    }
}
