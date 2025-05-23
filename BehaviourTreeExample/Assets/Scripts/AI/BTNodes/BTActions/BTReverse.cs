using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTReverse : BTDecorator
{
    public BTReverse(BTBaseNode child) : base(child)
    {

    }

    protected override TaskStatus OnUpdate()
    {
        var result = child.Tick();

        switch (result)
        {
            case TaskStatus.Success: return TaskStatus.Failed;
            case TaskStatus.Failed: return TaskStatus.Success;
            case TaskStatus.Running: return TaskStatus.Running;
        }
        return TaskStatus.Running;
    }
}
