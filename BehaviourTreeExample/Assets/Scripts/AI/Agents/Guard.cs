using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float stoppingDistance = 0.1f;
    [SerializeField] private GameObject player;
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private Transform weapon;

    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Animator animator;
    private Blackboard blackboard = new Blackboard();

    // private static event Func<bool> TestFunc;
    private static event Func<bool> SeesPlayer;
    private static event Func<bool> HasWeapon;
    private static event Func<bool> IsSearching;
    private static event Func<bool> InAttackRange;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        // TestFunc += ReturnFalse;
        SeesPlayer += GuardSeesPlayer;
        HasWeapon += GuardHasWeapon;
        IsSearching += GuardIsSearching;
        InAttackRange += GuardInAttackRange;
    }

    private void Start()
    {
        //Create your Behaviour Tree here!
        blackboard.SetVariable(VariableNames.TARGET_POSITION_A, waypoints[0].position);
        blackboard.SetVariable(VariableNames.TARGET_POSITION_B, waypoints[2].position);
        blackboard.SetVariable(VariableNames.TARGET_POSITION_C, waypoints[2].position);
        blackboard.SetVariable(VariableNames.TARGET_POSITION_D, waypoints[3].position);
        blackboard.SetVariable(VariableNames.TARGET_POSITION_WEAPON, weapon.position);

        blackboard.SetVariable(VariableNames.HAS_WEAPON, false);
        blackboard.SetVariable(VariableNames.STATE, State.PATROLLING);
        blackboard.SetVariable(VariableNames.SEES_PLAYER, false);

        tree = new BTSelector(
            new BTConditionalDecorator(IsSearching, new BTSequence(
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_WEAPON, stoppingDistance),
                new BTGrab())),

            new BTConditionalDecorator(InAttackRange, new BTAttack()),

            new BTConditionalDecorator(SeesPlayer, new BTSelector(
                new BTConditionalDecorator(HasWeapon, new BTStartSearch()),
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_PLAYER, stoppingDistance))),

            new BTSequence(
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_A, stoppingDistance),
                new BTWait(1f),
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_B, stoppingDistance),
                new BTWait(1f),
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_C, stoppingDistance),
                new BTWait(1f),
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_D, stoppingDistance),
                new BTWait(1f)
            ));
        tree.SetupBlackboard(blackboard);
    }

    private void FixedUpdate()
    {
        SeePlayerCheck();
        TaskStatus result = tree.Tick();
        blackboard.SetVariable(VariableNames.TARGET_POSITION_PLAYER, player.transform.position);
        Debug.Log(blackboard.GetVariable<State>(VariableNames.STATE));
        Debug.Log(blackboard.GetVariable<Vector3>(VariableNames.TARGET_POSITION_WEAPON));
    }

    // Used for testing func / conditional decorator
    private bool CoinFlip()
    {
        float coin = UnityEngine.Random.Range(0f, 1f);
        if (coin > 0.5f)
        {
            return true;
        }
        else { return false; }
    }

    // Used for testing func / conditional decorator
    private bool ReturnFalse()
    {
        return false;
    }

    // Will use a raycast or the like eventually, currently used for easy testing
    private bool GuardSeesPlayer()
    {
        if (player.transform.position.z > 0f)
        {
            blackboard.SetVariable(VariableNames.SEES_PLAYER, true);
            return true;
        }
        blackboard.SetVariable(VariableNames.SEES_PLAYER, false);
        return false;
    }

    // I'm not sure which of these two functions is redundant, probably the one above^,
    // maybe I could make it a simple funciton that returns true/false only based on the variable?
    // This function however seems necessary in Update.
    private void SeePlayerCheck()
    {
        if (player.transform.position.z > 0f)
        {
            blackboard.SetVariable(VariableNames.SEES_PLAYER, true);
        }
        else { blackboard.SetVariable(VariableNames.SEES_PLAYER, false); }
    }

    // This function is a bit backwards, since the condition is met when the 
    // guard has NO weapon. Might have to chnage the variable name.
    private bool GuardHasWeapon()
    {
        if (blackboard.GetVariable<bool>(VariableNames.HAS_WEAPON))
        {
            return false;
        }
        return true;
    }

    private bool GuardIsSearching()
    {
        if (blackboard.GetVariable<bool>(VariableNames.IS_SEARCHING))
        {
            return true;
        }
        return false;
    }

    private bool GuardInAttackRange()
    {
        if ((player.transform.position - transform.position).magnitude < 1f)
        {
            return true;
        }
        return false;
    }
}
