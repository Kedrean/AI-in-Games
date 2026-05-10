using UnityEngine;
using UnityEngine.AI;

public class Patrol : MonoBehaviour
{
    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    private int currentPointIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            GoToNextPoint();
        }
    }

    void GoToNextPoint()
    {
        currentPointIndex++;

        if (currentPointIndex >= patrolPoints.Length)
        {
            currentPointIndex = 0;
        }

        agent.SetDestination(patrolPoints[currentPointIndex].position);
    }
}
