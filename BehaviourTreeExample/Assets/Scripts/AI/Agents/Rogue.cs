using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class Rogue : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float stoppingDistance = 3;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform cover; 
    [SerializeField] private Guard guard;
    [SerializeField] private TextMeshProUGUI stateUI;

    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Blackboard blackboard = new Blackboard();

    private static event Func<bool> PlayerInDanger;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        PlayerInDanger += IsPlayerInDanger;
    }

    private void Start()
    {
        //TODO: Create your Behaviour tree here
        blackboard.SetVariable(VariableNames.STATE, State.FOLLOWING);
        blackboard.SetVariable(VariableNames.TARGET_POSITION_COVER, cover.position);

        tree = new BTSelector(
            new BTConditionalDecorator(PlayerInDanger, new BTParallel(
                new BTLog("Defending", stateUI),
                new BTSequence(
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_COVER, stoppingDistance),
                    new BTWait(0.5f),
                    new BTStunEnemy(guard),
                    new BTWait(5)
                )
            )),

            new BTParallel(
                new BTLog("Following", stateUI),
                new BTReverse(new BTCheckCondition(PlayerInDanger)),
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_PLAYER, stoppingDistance)
                ));
        tree.SetupBlackboard(blackboard);
    }

    private void FixedUpdate()
    {
        blackboard.SetVariable(VariableNames.TARGET_POSITION_PLAYER, player.transform.position);
        tree?.Tick();
    }

    private bool IsPlayerInDanger()
    {
        if (guard.IsGuardAttacking())
        {
            return true;
        }
        return false;
    }
}
