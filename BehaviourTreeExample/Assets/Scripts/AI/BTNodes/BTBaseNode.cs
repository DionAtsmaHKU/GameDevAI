﻿using System;
using UnityEngine;

public enum TaskStatus { Success, Failed, Running }
public abstract class BTBaseNode
{
    //Members
    protected Blackboard blackboard;
    private bool wasEntered = false;

    //Public Methods
    public virtual void OnReset() { }

    public TaskStatus Tick()
    {
        if (!wasEntered)
        {
            OnEnter();
            wasEntered = true;
        }

        var result = OnUpdate();
        if(result != TaskStatus.Running)
        {
            OnExit();
            wasEntered = false;
        }
        return result;
    }

    public virtual void SetupBlackboard(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }

    //Protected Methods
    protected abstract TaskStatus OnUpdate();
    protected virtual void OnEnter() { }
    protected virtual void OnExit() { }
}

public abstract class BTComposite : BTBaseNode
{
    protected BTBaseNode[] children;
    public BTComposite (params BTBaseNode[] children)
    {
        this.children = children;
    }

    public override void SetupBlackboard(Blackboard blackboard)
    {
        base.SetupBlackboard(blackboard);
        foreach(BTBaseNode node in children)
        {
            node.SetupBlackboard(blackboard);
        }
    }
}

public abstract class BTDecorator : BTBaseNode
{
    protected BTBaseNode child;
    public BTDecorator(BTBaseNode child)
    {
        this.child = child;
    }

    public override void SetupBlackboard(Blackboard blackboard)
    {
        base.SetupBlackboard(blackboard);
        child.SetupBlackboard(blackboard);
    }
}

// Used for conditional logic in the behaviour tree.
public class BTConditionalDecorator : BTDecorator
{
    private Func<bool> condition;

    public BTConditionalDecorator(Func<bool> condition, BTBaseNode child) : base(child)
    {
        this.child = child;
        this.condition = condition;
    }

    protected override TaskStatus OnUpdate()
    {
        if (condition())
        {
            return child.Tick();
        }
        else
        {
            return TaskStatus.Failed;
        }
    }
}
