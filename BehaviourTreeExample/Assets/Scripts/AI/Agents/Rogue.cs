using System;
using TMPro;
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
        // Blackboard setup
        blackboard.SetVariable(VariableNames.TARGET_POSITION_COVER, cover.position);
        blackboard.SetVariable(VariableNames.TARGET_POSITION_PLAYER, player.transform);

        // Initialising Behaviour Tree
        tree = new BTSelector(
            
            // If the player is in danger, the rogue runs to cover and stuns the guard
            new BTConditionalDecorator(PlayerInDanger, new BTParallel(
                new BTLog("Defending", stateUI),
                new BTSequence(
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_COVER, stoppingDistance),
                    new BTWait(0.5f),
                    new BTStunEnemy(guard),
                    new BTWait(5)
                    )
                )
            ),

            // The rogue follows the player around
            new BTParallel(
                new BTLog("Following", stateUI),
                new BTReverse(new BTCheckCondition(PlayerInDanger)),
                new BTChasePlayer(agent, moveSpeed, stoppingDistance)
            )
        );
        tree.SetupBlackboard(blackboard);
    }

    private void FixedUpdate()
    {
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
