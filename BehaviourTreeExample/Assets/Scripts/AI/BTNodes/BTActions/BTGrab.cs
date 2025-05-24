using UnityEngine.AI;
using UnityEngine;

public class BTGrab : BTBaseNode
{
    private NavMeshAgent agent;
    private Vector3 targetPosition;
    private float keepDistance;
    private string targetString;

    public BTGrab(NavMeshAgent agent, string target, float keepDistance)
    {
        this.agent = agent;
        targetString = target;
        this.keepDistance = keepDistance;
    }

    protected override void OnEnter()
    {
        targetPosition = blackboard.GetVariable<Vector3>(targetString);
    }

    protected override TaskStatus OnUpdate()
    {
        // Grab the weapon if in range
        if (Vector3.Distance(agent.transform.position, targetPosition) <= keepDistance)
        {
            blackboard.SetVariable(VariableNames.HAS_WEAPON, true);
            return TaskStatus.Success;
        }
        return TaskStatus.Failed;
    }
}
