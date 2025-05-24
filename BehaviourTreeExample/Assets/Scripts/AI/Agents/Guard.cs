using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

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

    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Blackboard blackboard = new Blackboard();

    private static event Func<bool> SeesPlayer;
    private static event Func<bool> HasWeapon;
    private static event Func<bool> CanAttack;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        SeesPlayer += GuardSeesPlayer;
        HasWeapon += GuardHasWeapon;
        CanAttack += GuardCanAttack;
    }

    private void Start()
    {
        // Blackboard setup
        blackboard.SetVariable(VariableNames.TARGET_POSITION_A, waypoints[0].position);
        blackboard.SetVariable(VariableNames.TARGET_POSITION_B, waypoints[2].position);
        blackboard.SetVariable(VariableNames.TARGET_POSITION_C, waypoints[2].position);
        blackboard.SetVariable(VariableNames.TARGET_POSITION_D, waypoints[3].position);
        blackboard.SetVariable(VariableNames.TARGET_POSITION_WEAPON, weapon.position);

        blackboard.SetVariable(VariableNames.HAS_WEAPON, false);
        blackboard.SetVariable(VariableNames.IS_STUNNED, false);
        blackboard.SetVariable(VariableNames.SEES_PLAYER, false);
        blackboard.SetVariable(VariableNames.PLAYER_DEAD, false);
        blackboard.SetVariable(VariableNames.TARGET_POSITION_PLAYER, player.transform);

        // Initialising Behaviour Tree
        tree = new BTSelector(

            // Attack if the player is in range
            new BTConditionalDecorator(CanAttack, new BTAttack()),

            // If it sees the player, search for a weapon, if it has a weapon, chase player
            new BTConditionalDecorator(SeesPlayer, new BTSelector(
                new BTConditionalDecorator(HasWeapon, new BTParallel(
                    new BTLog("Chasing", stateUI),
                    new BTChasePlayer(agent, moveSpeed, stoppingDistance))
                ),
                new BTParallel(
                    new BTLog("Searching", stateUI),
                    new BTSequence(
                        new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_WEAPON, stoppingDistance),
                        new BTGrab(agent, VariableNames.TARGET_POSITION_WEAPON, stoppingDistance)
                    ))
                )
            ),

            // Patrolling, gets interrupted as soon as the guard sees the player.
            new BTParallel(
                new BTLog("Patrolling", stateUI),
                new BTReverse(new BTCheckCondition(SeesPlayer)),
                new BTSequence(
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_A, stoppingDistance),
                    new BTWait(1f),
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_B, stoppingDistance),
                    new BTWait(1f),
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_C, stoppingDistance),
                    new BTWait(1f),
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_D, stoppingDistance),
                    new BTWait(1f)
                )
            )
        ); 
        tree.SetupBlackboard(blackboard);
    }

    private void FixedUpdate()
    {
        SpotPlayer();
        TaskStatus result = tree.Tick();
        if (result != TaskStatus.Running)
        {
            tree.OnReset();
        }
    }

    public void Stun()
    {
        StartCoroutine(StunGuard());
    }

    // The guard cannot move for stunTime seconds, before regaining its speed
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

    // Used for Ally to determine whether the guard is attacking
    public bool IsGuardAttacking()
    {
        if (GuardSeesPlayer())
        {
            return true;
        }
        return false;
    }

    // Sets SEES_PLAYER to true if the guard sees the player with a Raycast.
    private bool SpotPlayer()
    {
        if (blackboard.GetVariable<bool>(VariableNames.PLAYER_DEAD))
        {
            blackboard.SetVariable(VariableNames.SEES_PLAYER, false);
            return false;
        }

        if (Physics.Raycast(head.position, player.transform.position - head.position, out RaycastHit hit, 1000, playerLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                blackboard.SetVariable(VariableNames.SEES_PLAYER, true);
                return true;
            }
        }

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

    private bool GuardHasWeapon()
    {
        if (blackboard.GetVariable<bool>(VariableNames.HAS_WEAPON))
        {
            return true;
        }
        return false;
    }

    private bool GuardCanAttack()
    {
        if ((player.transform.position - transform.position).magnitude < 1f && 
            blackboard.GetVariable<bool>(VariableNames.HAS_WEAPON) &&
            !blackboard.GetVariable<bool>(VariableNames.PLAYER_DEAD))
        {
            return true;
        }
        return false;
    }
}
