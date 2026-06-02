using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    public Transform[] destination;
    public int currentWaypointIndex = 0;

    public StopLight trafficLight;
    public Transform stopPoint;
    public float stopDistance = 2f;

    public float normalSpeed;
    public float slowSpeed;

    private NavMeshAgent carAgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        carAgent = GetComponent<NavMeshAgent>();
        carAgent.speed = normalSpeed;

        MoveToCurrentWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        FollowTrafficLight();

        if (carAgent.pathPending)
        {
            return;
        }

        if (carAgent.remainingDistance <= carAgent.stoppingDistance)
        {
            GoToNextWaypoint();
        }
    }

    private void FollowTrafficLight()
    {
        float distanceToStopPoint = Vector3.Distance(transform.position, stopPoint.position);

        if (!trafficLight.isGreen && distanceToStopPoint <= stopDistance * 2)
        {
            carAgent.isStopped = true;
            carAgent.speed = 0;
        }

        else if (!trafficLight.isGreen && distanceToStopPoint <= stopDistance * 4)
        {
            carAgent.isStopped = false;
            carAgent.speed = slowSpeed;
        }

        else
        {
            carAgent.isStopped = false;
            carAgent.speed = normalSpeed;
        }
    }

    private void MoveToCurrentWaypoint()
    {
        carAgent.SetDestination(destination[currentWaypointIndex].position);
    }

    private void GoToNextWaypoint()
    {
        currentWaypointIndex++;

        if (currentWaypointIndex >= destination.Length)
        {
            currentWaypointIndex = 0;
        }

        MoveToCurrentWaypoint();
    }
}
