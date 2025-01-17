using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class Rogue : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float stoppingDistance = 3;
    [SerializeField] private GameObject player;

    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Animator animator;
    private Blackboard blackboard = new Blackboard();

    private static event Func<bool> TestFunc;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        TestFunc += ReturnFalse;
    }

    private void Start()
    {
        //TODO: Create your Behaviour tree here
        blackboard.SetVariable(VariableNames.STATE, State.CHASING);

        tree = new BTSelector(
            new BTConditionalDecorator(TestFunc, new BTAttack()), // Still need to implement smoke attack

            new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_PLAYER, stoppingDistance)
            );
        tree.SetupBlackboard(blackboard);
    }
    private void FixedUpdate()
    {
        blackboard.SetVariable(VariableNames.TARGET_POSITION_PLAYER, player.transform.position);
        tree?.Tick();
    }

    // Used for testfunc
    private bool ReturnFalse()
    {
        return false;
    }
}
