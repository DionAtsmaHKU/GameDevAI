using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTAttack : BTBaseNode
{
    public BTAttack()
    {

    }

    protected override void OnEnter()
    {
        blackboard.SetVariable(VariableNames.STATE, State.ATTACKING);
    }

    protected override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
