using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] destination;
    public int currentWaypointIndex = 0;

    [Header("Traffic Light")]
    public StopLight trafficLight;
    public Transform stopPoint;
    public float stopDistance = 2f;

    [Header("Car Speeds")]
    public float normalSpeed = 5f;
    public float slowSpeed = 2.5f;

    [Header("Car Detection")]
    public float carDetectionDistance = 4f;
    public float carDetectionRadius = 0.6f;
    public LayerMask carLayer;

    private NavMeshAgent carAgent;

    void Start()
    {
        carAgent = GetComponent<NavMeshAgent>();
        carAgent.speed = normalSpeed;

        MoveToCurrentWaypoint();
    }

    void Update()
    {
        FollowTrafficLight();

        // Don't continue if stopped
        if (carAgent.isStopped)
            return;

        if (carAgent.pathPending)
            return;

        if (carAgent.remainingDistance <= carAgent.stoppingDistance)
        {
            GoToNextWaypoint();
        }
    }

    private void FollowTrafficLight()
    {
        // Stop if another car is directly ahead
        if (IsCarAhead())
        {
            carAgent.isStopped = true;
            carAgent.speed = 0;
            return;
        }

        float distanceToStopPoint = Vector3.Distance(transform.position, stopPoint.position);

        // Red light - stop
        if (!trafficLight.isGreen && distanceToStopPoint <= stopDistance * 2)
        {
            carAgent.isStopped = true;
            carAgent.speed = 0;
        }
        // Red light - slow down
        else if (!trafficLight.isGreen && distanceToStopPoint <= stopDistance * 4)
        {
            carAgent.isStopped = false;
            carAgent.speed = slowSpeed;
        }
        // Green light - normal speed
        else
        {
            carAgent.isStopped = false;
            carAgent.speed = normalSpeed;
        }
    }

    private bool IsCarAhead()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.SphereCast(
                origin,
                carDetectionRadius,
                transform.forward,
                out RaycastHit hit,
                carDetectionDistance,
                carLayer))
        {
            // Ignore ourselves
            if (hit.transform != transform)
            {
                return true;
            }
        }

        return false;
    }

    private void MoveToCurrentWaypoint()
    {
        if (destination.Length == 0)
            return;

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Gizmos.DrawLine(origin, origin + transform.forward * carDetectionDistance);
        Gizmos.DrawWireSphere(origin + transform.forward * carDetectionDistance, carDetectionRadius);
    }
}