using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTGrab : BTBaseNode
{
    public BTGrab()
    {

    }

    protected override TaskStatus OnUpdate()
    {
        Debug.Log("As the kids say: Yoink");
        return TaskStatus.Success;
    }
}
