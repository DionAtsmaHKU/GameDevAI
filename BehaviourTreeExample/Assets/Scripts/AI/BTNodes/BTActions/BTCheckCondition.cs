using System;

public class BTCheckCondition : BTBaseNode
{
    Func<bool> condition;

    public BTCheckCondition(Func<bool> c)
    {
        condition = c;
    }

    protected override TaskStatus OnUpdate()
    {
        if (condition())
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failed;
        }
    }
}
