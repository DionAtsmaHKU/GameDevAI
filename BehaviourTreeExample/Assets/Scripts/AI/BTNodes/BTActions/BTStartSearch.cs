using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTStartSearch : BTBaseNode
{
    public BTStartSearch()
    {

    }

    protected override void OnEnter()
    {

    }

    protected override TaskStatus OnUpdate()
    {
        blackboard.SetVariable(VariableNames.STATE, State.SEARCHING);
        return TaskStatus.Success;
    }
}
