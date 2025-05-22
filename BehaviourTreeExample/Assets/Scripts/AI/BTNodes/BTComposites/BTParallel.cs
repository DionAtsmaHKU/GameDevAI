using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTParallel : BTComposite
{
    public BTParallel(params BTBaseNode[] children) : base(children) { }

    protected override TaskStatus OnUpdate()
    {
        for (int currentIndex = 0; currentIndex < children.Length; currentIndex++)
        {
            var result = children[currentIndex].Tick();
            switch (result)
            {
                case TaskStatus.Success: continue;
                case TaskStatus.Failed: return TaskStatus.Failed;
                case TaskStatus.Running: continue;
            }
        }
        return TaskStatus.Running;
    }

    protected override void OnEnter()
    {

    }

    protected override void OnExit()
    {

    }

    public override void OnReset()
    {
        foreach (var c in children)
        {
        c.OnReset();
        }
    }
}
