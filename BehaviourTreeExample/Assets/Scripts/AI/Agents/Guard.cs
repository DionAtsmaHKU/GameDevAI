using System;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float stoppingDistance = 0.1f;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        TestFunc += CoinFlip;
    }

    private void Start()
    {
        //Create your Behaviour Tree here!
        Blackboard blackboard = new Blackboard();
        blackboard.SetVariable(VariableNames.TARGET_POSITION, new Vector3(0, 0, 0));
        blackboard.SetVariable(VariableNames.TARGET_POSITION_ALTERNATE, new Vector3(3, 0, 0));

        tree = new BTSequence(
            new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION, stoppingDistance),
            new BTWait(1f),
            new BTConditionalDecorator(new BTLog("Hoi"), TestFunc),
            new BTMoveToPosition(agent, moveSpeed, VariableNames.TARGET_POSITION_ALTERNATE, stoppingDistance),
            new BTWait(1f)
            );
        tree.SetupBlackboard(blackboard);
    }


    private void FixedUpdate()
    {
        TaskStatus result = tree.Tick();
    }

    private static event Func<bool> TestFunc;

    private bool CoinFlip()
    {
        float coin = UnityEngine.Random.Range(0f, 1f);
        if (coin > 0.5f)
        {
            return true;
        }
        else { return false; }
       
    }
}
