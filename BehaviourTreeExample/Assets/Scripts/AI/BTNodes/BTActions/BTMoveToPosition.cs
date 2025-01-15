using System.Collections;
using System.Collections.Generic;
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
        if (BBtargetPosition == VariableNames.TARGET_POSITION_PLAYER)
        {
            blackboard.SetVariable(VariableNames.STATE, State.CHASING);
        } else { blackboard.SetVariable(VariableNames.STATE, State.PATROLLING); }
    }

    protected override TaskStatus OnUpdate()
    {
        if (agent == null || (blackboard.GetVariable<bool>(VariableNames.SEES_PLAYER) 
                              && blackboard.GetVariable<State>(VariableNames.STATE) == State.PATROLLING)) 
                              { Debug.Log("STOP PATROLLIGN NOWWWWWWWWWW"); return TaskStatus.Failed; }
        if (agent.pathPending) { return TaskStatus.Running; }
        if (agent.hasPath && agent.path.status == NavMeshPathStatus.PathInvalid) { Debug.Log("Invalid path"); return TaskStatus.Failed; }
        if (agent.pathEndPosition != targetPosition)
        {
            agent.SetDestination(targetPosition);
        }

        if(Vector3.Distance(agent.transform.position, targetPosition) <= keepDistance)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;

    }
}
