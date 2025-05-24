using UnityEngine;
using UnityEngine.AI;

public class BTMoveToPosition : BTBaseNode
{
    private NavMeshAgent agent;
    private float moveSpeed;
    private float keepDistance;
    private Vector3 targetPosition;
    private string BBtargetPosition;

    public BTMoveToPosition(NavMeshAgent agent, float moveSpeed, string BBtargetPosition, float keepDistance)
    {
        this.agent = agent;
        this.moveSpeed = moveSpeed;
        this.BBtargetPosition = BBtargetPosition;
        this.keepDistance = keepDistance;
    }

    protected override void OnEnter()
    {
        agent.speed = moveSpeed;
        agent.stoppingDistance = keepDistance;
        targetPosition = blackboard.GetVariable<Vector3>(BBtargetPosition);
    }

    protected override TaskStatus OnUpdate()
    {
        if (agent == null)
            return TaskStatus.Failed;

        if (agent.pathPending)
            return TaskStatus.Running;

        if (agent.hasPath && agent.path.status == NavMeshPathStatus.PathInvalid) { Debug.Log("Invalid path"); return TaskStatus.Failed; }

        if (agent.pathEndPosition != targetPosition)
            agent.SetDestination(targetPosition);

        if (Vector3.Distance(agent.transform.position, targetPosition) <= keepDistance)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}

// Similar to MoveToPosition, but instead uses a Transform for chasing instead of a static position.
public class BTChasePlayer : BTBaseNode
{
    private NavMeshAgent agent;
    private float moveSpeed;
    private float keepDistance;
    private Vector3 targetPosition;

    public BTChasePlayer(NavMeshAgent agent, float moveSpeed, float keepDistance)
    {
        this.agent = agent;
        this.moveSpeed = moveSpeed;
        this.keepDistance = keepDistance;
    }

    protected override void OnEnter()
    {
        agent.speed = moveSpeed;
        agent.stoppingDistance = keepDistance;
        targetPosition = blackboard.GetVariable<Transform>("TARGET_POSITION_PLAYER").position;
    }

    protected override TaskStatus OnUpdate()
    {
        if (agent == null)
           return TaskStatus.Failed;

        targetPosition = blackboard.GetVariable<Transform>("TARGET_POSITION_PLAYER").position;

        if (agent.pathPending) 
            return TaskStatus.Running;

        if (agent.hasPath && agent.path.status == NavMeshPathStatus.PathInvalid) { Debug.Log("Invalid path"); return TaskStatus.Failed; }

        if (agent.pathEndPosition != targetPosition)
            agent.SetDestination(targetPosition);

        if (Vector3.Distance(agent.transform.position, targetPosition) <= keepDistance)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
