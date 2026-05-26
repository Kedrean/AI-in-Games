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

    [Header("Attack")]
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float attackCooldown = 1.2f;
    [SerializeField] string attackTrigger = "Attack";

    private NavMeshAgent agent;
    private Animator animator;
    private EnemyStates currentState;
    private bool stateInitialized;
    private int patrolIndex;
    private float waitTimer;
    private float lastAttackTime;

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

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

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
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (player == null)
        {
            if (HasPatrolPoints)
            {
                ChangeState(EnemyStates.Patrol);
            }

            else
            {
                ChangeState(EnemyStates.Idle);
            }

            return;
        }

        if (distanceToPlayer <= chaseStopDistance)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    private void EnterAttack()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    private void UpdateAttack()
    {
        if (player == null)
        return;

        // Face player smoothly
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                facePlayerSpeed * Time.deltaTime
            );
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Return to chase if player escaped
        if (distanceToPlayer > attackRange)
        {
            ChangeState(EnemyStates.Chase);
            return;
        }

        // Play attack animation
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
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
}
