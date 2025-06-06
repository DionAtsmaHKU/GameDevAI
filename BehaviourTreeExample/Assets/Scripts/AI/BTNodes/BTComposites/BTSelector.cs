public class BTSelector : BTComposite
{
    private int currentIndex = 0;

    public BTSelector(params BTBaseNode[] children) : base(children) { }

    protected override TaskStatus OnUpdate()
    {
        for (currentIndex = 0; currentIndex < children.Length; currentIndex++)
        {
            var result = children[currentIndex].Tick();
            switch (result)
            {
                case TaskStatus.Success: return TaskStatus.Success;
                case TaskStatus.Failed: continue;
                case TaskStatus.Running: return TaskStatus.Running;
            }
        }
        return TaskStatus.Failed;
    }

    protected override void OnEnter()
    {
        currentIndex = 0;
    }

    protected override void OnExit()
    {
        currentIndex = 0;
    }

    public override void OnReset()
    {
        currentIndex = 0;
        foreach (var c in children)
        {
            c.OnReset();
        }
    }
}
