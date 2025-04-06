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

    // the start of this function all seems a little overcomplicated, I'll have to see if there's a better way
    // of handeling the different types of moving (to a static/moving point).
    protected override TaskStatus OnUpdate()
    {
        // This needs to become an interrupt!!!
        if (agent == null || (blackboard.GetVariable<bool>(VariableNames.SEES_PLAYER) 
                              && blackboard.GetVariable<State>(VariableNames.STATE) == State.PATROLLING))
                              { Debug.Log("Stop patrolling NOW"); return TaskStatus.Failed; }

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
        targetPosition = blackboard.GetVariable<Vector3>("TARGET_POSITION_PLAYER");
    }

    // the start of this function all seems a little overcomplicated, I'll have to see if there's a better way
    // of handeling the different types of moving (to a static/moving point).
    protected override TaskStatus OnUpdate()
    {
        // This needs to become an interrupt!!!
        if (agent == null)
           return TaskStatus.Failed;

        targetPosition = blackboard.GetVariable<Vector3>("TARGET_POSITION_PLAYER");

        if (agent.pathPending) 
            return TaskStatus.Running;

        if (agent.hasPath && agent.path.status == NavMeshPathStatus.PathInvalid) { Debug.Log("Invalid path"); return TaskStatus.Failed; }

        if (agent.pathEndPosition != targetPosition)
            agent.SetDestination(targetPosition);

        if (Vector3.Distance(agent.transform.position, targetPosition) <= keepDistance)
        {
            Debug.Log("CHASING PLAYER WOOOOO YAEH");
            return TaskStatus.Success;
        }
        return TaskStatus.Running;

    }
}
