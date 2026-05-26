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

    [Header("Attack")]
    [SerializeField] string attackTrigger = "Attack";
    [SerializeField] float attackCooldown = 1.2f;
    [SerializeField] private float attackRange = 0.75f;

    [Header("Animation")]
    [SerializeField] string speedParam = "Speed";
    [SerializeField] float animDampTime = 0.1f;

    private NavMeshAgent agent;
    private Animator animator;
    private EnemyStates currentState;
    private bool stateInitialized;
    private int patrolIndex;
    private float waitTimer;
    private float attackTimer;

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
        Chase,
        Attack
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
            ChangeState(EnemyStates.Patrol);
        }

        else
        {
            ChangeState(EnemyStates.Idle);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayer();
        UpdateAnimation();

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
            case EnemyStates.Attack:
                UpdateAttack();
                break;
            default:
                break;
        }
    }

    private void CheckForPlayer()
    {
        if (player == null)
            return;

        float distanceToPlayer = DistanceToPlayerXZ();

        if (distanceToPlayer <= attackRange)
        {
            ChangeState(EnemyStates.Attack);
        }

        else if (currentState != EnemyStates.Chase && distanceToPlayer <= chaseRange)
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
            case EnemyStates.Attack:
                EnterAttack();
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

        agent.ResetPath();
        SetCurrentPatrolDestination();
    }

    private void UpdatePatrol()
    {
        if (!HasPatrolPoints)
        {
            ChangeState(EnemyStates.Idle);
        }

        if (!ReachedDestination())
        {
            return;
        }

        agent.isStopped = true;

        waitTimer += Time.deltaTime;
        if (waitTimer >= waitTimeAtWaypoin)
        {
            waitTimer = 0;
            ChooseNextPatrolPoint();
            agent.isStopped = false;
            SetCurrentPatrolDestination();
        }
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
        if (player == null)
        {
            ChangeState(HasPatrolPoints ? EnemyStates.Patrol : EnemyStates.Idle);
            return;
        }

        float distanceToPlayer = DistanceToPlayerXZ();

        // Always follow player first
        agent.SetDestination(player.position);

        // Stop near player
        if (distanceToPlayer <= chaseStopDistance)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            ChangeState(EnemyStates.Attack);
        }

        else
        {
            agent.isStopped = false;
        }
    }

    private void EnterAttack()
    {
        agent.isStopped = true;
        agent.ResetPath();

        attackTimer = 0;

        animator.ResetTrigger(attackTrigger);
        animator.SetTrigger(attackTrigger);
    }

    private void UpdateAttack()
    {
        if (player == null)
            return;

        // Face the player
        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                facePlayerSpeed * Time.deltaTime
            );
        }

        float distanceToPlayer = DistanceToPlayerXZ();

        // Player moved away
        if (distanceToPlayer > attackRange)
        {
            ChangeState(EnemyStates.Chase);
            return;
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0;

            animator.ResetTrigger(attackTrigger);
            animator.SetTrigger(attackTrigger);
        }
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
        if (!HasPatrolPoints)
        {
            return;
        }

        if (randomPatrol && patrolPoints.Length > 1)
        {
            int nextIndex = patrolIndex;
            while (nextIndex == patrolIndex)
            {
                nextIndex = Random.Range(0, patrolPoints.Length);
            }
            patrolIndex = nextIndex;
        }
        else
        {
            patrolIndex++;

            if (patrolIndex >= patrolPoints.Length)
            {
                patrolIndex = 0;
            }
        }
    }

    private void UpdateAnimation()
    {
        float animationSpeed = 0;

        bool isMoving = agent.velocity.magnitude > 0.5 && !agent.isStopped;
        if (currentState == EnemyStates.Patrol && isMoving)
        {
            animationSpeed = 0.5f;
        }

        else if (currentState == EnemyStates.Chase && isMoving)
        {
            animationSpeed = 1.0f;
        }

        animator.SetFloat(speedParam, animationSpeed, animDampTime, Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);

        if (patrolPoints == null)
            return;

        Gizmos.color = Color.yellow;
        foreach (Transform point in patrolPoints)
        {
            Gizmos.DrawLine(transform.position, point.position);
        }
    }

    private float DistanceToPlayerXZ()
    {
        Vector2 enemyPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 playerPos = new Vector2(player.position.x, player.position.z);

        return Vector2.Distance(enemyPos, playerPos);
    }
}
