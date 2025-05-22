using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
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
    [SerializeField] private TextMeshProUGUI stateUI;

    [Range(1, 360)] public float angle = 180f;

    private Vector3 offset = new Vector3(0, -10, 0);
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Animator animator;
    private Blackboard blackboard = new Blackboard();

    // private static event Func<bool> TestFunc;
    private static event Func<bool> SeesPlayer;
    private static event Func<bool> NoPlayer;
    private static event Func<bool> HasWeapon;
    private static event Func<bool> InAttackRange;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        // TestFunc += ReturnFalse;
        SeesPlayer += GuardSeesPlayer;
        NoPlayer += GuardNoPlayer;
        HasWeapon += GuardHasWeapon;
        InAttackRange += GuardInAttackRange;

        BTBaseNode.onStateChanged += UpdateUI;
    }

    private void OnDestroy()
    {
        BTBaseNode.onStateChanged -= UpdateUI;
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
        blackboard.SetVariable(VariableNames.PLAYER_DEAD, false);

        tree = new BTSelector(
            // Attack
            new BTConditionalDecorator(InAttackRange, new BTAttack()),

            // If it sees the player, search for a weapon, if it has a weapon, chase player
            new BTConditionalDecorator(SeesPlayer, new BTSelector(
                new BTConditionalDecorator(HasWeapon, new BTSequence(
                    new BTChasePlayer(agent, moveSpeed, stoppingDistance))
                ),
                new BTSequence(
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_WEAPON, stoppingDistance),
                    new BTGrab(agent, VariableNames.TARGET_POSITION_WEAPON, stoppingDistance)
                ))
            ),
            
            // Patrolling (if it doesnt see the player)
             new BTConditionalDecorator(NoPlayer, new BTSequence(
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_A, stoppingDistance),
                new BTWait(1f),
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_B, stoppingDistance),
                new BTWait(1f),
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_C, stoppingDistance),
                new BTWait(1f),
                new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_D, stoppingDistance),
                new BTWait(1f)
             ))
        ); 
        tree.SetupBlackboard(blackboard);
    }

    private void FixedUpdate()
    {
        SpotPlayerLinecast();
        TaskStatus result = tree.Tick();
        if (result != TaskStatus.Running)
        {
            tree.OnReset();
        }

        blackboard.SetVariable(VariableNames.TARGET_POSITION_PLAYER, player.transform.position); // Make this a transform reference in blackboard vars
        //Debug.Log(blackboard.GetVariable<State>(VariableNames.STATE));
        // stateUI.text = "Current State: \n" + blackboard.GetVariable<State>(VariableNames.STATE);
        //Debug.Log(blackboard.GetVariable<Vector3>(VariableNames.TARGET_POSITION_WEAPON));
    }

    private void UpdateUI(string nodeName)
    {
        stateUI.text = "Current Node: \n" + nodeName;
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

    private bool SpotPlayerLinecast()
    {
        if (blackboard.GetVariable<bool>(VariableNames.PLAYER_DEAD))
            return false;

        float distanceToTarget = Vector3.Distance(head.position, player.transform.position);

        //if (distanceToTarget > 5 && blackboard.GetVariable<State>(VariableNames.STATE) == State.CHASING)
        //{
        //    blackboard.SetVariable(VariableNames.SEES_PLAYER, false);
        //    return false;
        //}

        // Debug.Log(player.transform.position);

        if (Physics.Raycast(head.position, player.transform.position - head.position, out RaycastHit hit, 1000, playerLayer))
        {
            Debug.Log($"hit object: {hit.collider.gameObject.name}");
            Debug.DrawLine(head.position, player.transform.position);
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Sees player!!!!!!");
                blackboard.SetVariable(VariableNames.SEES_PLAYER, true);
                return true;
            }
            
        }
        Debug.DrawLine(head.position, player.transform.position);
        blackboard.SetVariable(VariableNames.SEES_PLAYER, false);
        return false;
    }

    private bool GuardSeesPlayer()
    {
        if (blackboard.GetVariable<bool>(VariableNames.SEES_PLAYER) &&
            !blackboard.GetVariable<bool>(VariableNames.PLAYER_DEAD))
        {
            return true;
        }
        return false;
    }

    private bool GuardNoPlayer()
    {
        if (blackboard.GetVariable<bool>(VariableNames.SEES_PLAYER) &&
            !blackboard.GetVariable<bool>(VariableNames.PLAYER_DEAD))
        {
            return false;
        }
        return true;
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
