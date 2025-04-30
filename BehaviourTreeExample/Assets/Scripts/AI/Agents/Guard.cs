using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Guard : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float stunTime = 3;
    [SerializeField] private float stoppingDistance = 0.1f;
    [SerializeField] private GameObject player;
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private Transform weapon;
    [SerializeField] private Transform head;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask wallLayer;

    [Range(1, 360)] public float angle = 180f;

    private Vector3 offset = new Vector3(0, -10, 0);
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Animator animator;
    private Blackboard blackboard = new Blackboard();

    // private static event Func<bool> TestFunc;
    private static event Func<bool> SeesPlayer;
    private static event Func<bool> HasWeapon;
    private static event Func<bool> InAttackRange;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        // TestFunc += ReturnFalse;
        SeesPlayer += SpotPlayerRaycast;
        HasWeapon += GuardHasWeapon;
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
        blackboard.SetVariable(VariableNames.IS_STUNNED, false);
        blackboard.SetVariable(VariableNames.STATE, State.PATROLLING);
        blackboard.SetVariable(VariableNames.SEES_PLAYER, false);

        tree = new BTSelector(
            new BTConditionalDecorator(InAttackRange, new BTAttack()),

            new BTConditionalDecorator(SeesPlayer, new BTSelector(
                new BTConditionalDecorator(HasWeapon, new BTSequence(
                    new BTSetState(State.CHASING),
                    new BTChasePlayer(agent, moveSpeed, stoppingDistance))
                ),
                new BTSequence(
                    new BTSetState(State.SEARCHING),
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_WEAPON, stoppingDistance),
                    new BTGrab(agent, VariableNames.TARGET_POSITION_WEAPON, stoppingDistance)
                ))
            ),

            // new BTRepeatUntilFail(
            new BTSequence(
                new BTSetState(State.PATROLLING),
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_A, stoppingDistance),
                new BTWait(1f),
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_B, stoppingDistance),
                new BTWait(1f),
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_C, stoppingDistance),
                new BTWait(1f),
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_D, stoppingDistance),
                new BTWait(1f)
            )
            //)
        ); 
        tree.SetupBlackboard(blackboard);
    }

    private void FixedUpdate()
    {
        // GuardSeesPlayer();
        SpotPlayerRaycast();
        TaskStatus result = tree.Tick();
        blackboard.SetVariable(VariableNames.TARGET_POSITION_PLAYER, player.transform.position);
        Debug.Log(blackboard.GetVariable<State>(VariableNames.STATE));
        Debug.Log(blackboard.GetVariable<Vector3>(VariableNames.TARGET_POSITION_WEAPON));
    }

    public void Stun()
    {
        StartCoroutine(StunGuard());
    }

    private IEnumerator StunGuard()
    {
        blackboard.SetVariable(VariableNames.IS_STUNNED, true);
        float oldSpeed = moveSpeed;
        moveSpeed = 0;
        agent.speed = 0;
        Debug.Log("GUARD STUNNED");
        yield return new WaitForSeconds(stunTime);
        blackboard.SetVariable(VariableNames.IS_STUNNED, false);
        agent.speed = oldSpeed;
        moveSpeed = oldSpeed;
    }

    public bool IsStunned()
    {
        if (blackboard.GetVariable<bool>(VariableNames.IS_STUNNED))
        {
            return true;
        }
        return false;
    }

    public bool IsGuardAttacking()
    {
        if (blackboard.GetVariable<State>(VariableNames.STATE) == State.CHASING ||
            blackboard.GetVariable<State>(VariableNames.STATE) == State.ATTACKING)
        {
            return true;
        }
        return false;
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

    private bool SpotPlayerRaycast()
    {
        if (blackboard.GetVariable<bool>(VariableNames.PLAYER_DEAD))
            return false;

        Vector3 directionToTarget = (player.transform.position - head.position).normalized;
        float distanceToTarget = Vector3.Distance(head.position, player.transform.position);

        //if (distanceToTarget > 5)
        //    return false;


        if (Physics.Raycast(head.position, directionToTarget, distanceToTarget, playerLayer))
        {
            if (!Physics.Raycast(head.position, directionToTarget, distanceToTarget, wallLayer))
            {
                blackboard.SetVariable(VariableNames.SEES_PLAYER, true);
                return true;
            }
        }
        blackboard.SetVariable(VariableNames.SEES_PLAYER, false);
        Debug.Log("no player??");
        return false;
    }

    private bool GuardHasWeapon()
    {
        if (blackboard.GetVariable<bool>(VariableNames.HAS_WEAPON))
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
