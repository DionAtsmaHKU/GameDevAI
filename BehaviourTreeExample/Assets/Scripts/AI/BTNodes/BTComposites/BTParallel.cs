using System.Collections.Generic;

public class BTParallel : BTComposite
{
    public BTParallel(params BTBaseNode[] children) : base(children) { }

    protected override TaskStatus OnUpdate()
    {
        // The results of the children's Tick()'s get stored in a List, so that 
        // the children all run at the same time before their results get processed.
        List<TaskStatus> results = new List<TaskStatus>();
        for (int currentIndex = 0; currentIndex < children.Length; currentIndex++)
        {
            var result = children[currentIndex].Tick();
            results.Add(result);
        }
        
        foreach(var result in results)
        {
            switch (result)
            {
                case TaskStatus.Success: continue;
                case TaskStatus.Failed: return TaskStatus.Failed;
                case TaskStatus.Running: return TaskStatus.Running;
            }
        }
        OnReset();
        return TaskStatus.Success;
    }
    
    public override void OnReset()
    {
        foreach (var c in children)
        {
        c.OnReset();
        }
    }
}
