using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] Transform[] patrolPoints;

    [Header("AI Settings")]
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float loseRange = 16f;

    [Header("Movement")]
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float patrolStopDistance = 0.2f;
    [SerializeField] float chaseStopDistance = 1f;
    [SerializeField] float waypointReachDistance = 0.5f;
    [SerializeField] float waitTimeAtWaypoin = 1.5f;
    [SerializeField] bool randomPatrol = false;
    [SerializeField] float facePlayerSpeed = 8f;

    [Header("Animation")]
    [SerializeField] string speedParam = "Speed";
    [SerializeField] float animDampTime = 0.1f;

    private NavMeshAgent agent;
    private Animator animator;
    private EnemyStates currentState;
    private bool stateInitialized;
    private int patrolIndex;
    private float waitTimer;

    private bool HasPatrolPoints
    {
        get
        {
            return patrolPoints != null && patrolPoints.Length > 0;
        }
    }

    private enum EnemyStates
    {
        Idle,
        Patrol,
        Chase
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (HasPatrolPoints)
        {
        
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayer();

        switch (currentState)
        {
            case EnemyStates.Idle:
                UpdateIdle();
                break;
            case EnemyStates.Patrol:
                UpdatePatrol();
                break;
            case EnemyStates.Chase:
                UpdateChase();
                break;
            default:
                break;
        }
    }

    private void CheckForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (currentState != EnemyStates.Chase && distanceToPlayer <= chaseRange)
        {
            ChangeState(EnemyStates.Chase);
        }

        else if (currentState == EnemyStates.Chase && distanceToPlayer >= loseRange)
        {
            ChangeState(EnemyStates.Patrol);
        }
    }

    private void ChangeState(EnemyStates newState)
    {
        if (stateInitialized && currentState == newState) 
            return;

        stateInitialized = true;

        currentState = newState;
        switch (currentState)
        {
            case EnemyStates.Idle:
                EnterIdle();
                break;
            case EnemyStates.Patrol:
                EnterPatrol();
                break;
            case EnemyStates.Chase:
                EnterChase();
                break;
        }
    }

    private void EnterIdle()
    {
        agent.isStopped = true;
        agent.ResetPath();
        waitTimer = 0;
    }

    private void UpdateIdle()
    {

    }

    private void EnterPatrol()
    {
        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.stoppingDistance = patrolStopDistance;
        waitTimer = 0;
    }

    private void UpdatePatrol()
    {

    }

    private void EnterChase()
    {
        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.stoppingDistance = chaseStopDistance;
        waitTimer = 0;
    }

    private void UpdateChase()
    {

    }

    private bool ReachedDestination()
    {
        if (agent.pathPending)
        {
            return false;
        }

        if (agent.remainingDistance == Mathf.Infinity)
        {
            return false; 
        }

        float reachedDistance = Mathf.Max(agent.stoppingDistance, waypointReachDistance);

        return agent.remainingDistance <= reachedDistance;
    }

    private void SetCurrentPatrolDestination()
    {
        if (!HasPatrolPoints) return;

        Transform point = patrolPoints[patrolIndex];

        if (point == null)
        {
            ChooseNextPatrolPoint();
            point = patrolPoints[patrolIndex];
        }

        if (point != null)
        {
            agent.SetDestination(point.position);
        }
    }

    private void ChooseNextPatrolPoint()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}
