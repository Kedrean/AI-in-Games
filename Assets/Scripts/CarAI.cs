using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    [Header("Route")]
    public Transform[] destination;

    private int currentWaypointIndex;

    [Header("Traffic Light")]
    public StopLight.Direction myDirection;
    public StopLight trafficLight;
    public Transform stopPoint;
    public float stopDistance = 2f;

    [Header("Car Speeds")]
    public float normalSpeed = 5f;
    public float slowSpeed = 2.5f;

    [Header("Car Detection")]
    public float carDetectionDistance = 5f;
    public float carDetectionRadius = 1f;
    public LayerMask carLayer;
    public Vector3 sensorOffset = new Vector3(0, 0.5f, 0);

    private NavMeshAgent carAgent;

    void Start()
    {
        carAgent = GetComponent<NavMeshAgent>();
        carAgent.speed = normalSpeed;

        if (destination != null && destination.Length > 0)
            MoveToCurrentWaypoint();
    }

    void Update()
    {
        FollowTrafficLight();

        if (carAgent.isStopped)
            return;

        if (carAgent.pathPending)
            return;

        if (carAgent.remainingDistance <= carAgent.stoppingDistance)
        {
            GoToNextWaypoint();
        }
    }

    public void SetRoute(Transform routeParent)
    {
        destination = new Transform[routeParent.childCount];

        for (int i = 0; i < routeParent.childCount; i++)
        {
            destination[i] = routeParent.GetChild(i);
        }

        currentWaypointIndex = 0;

        if (carAgent == null)
            carAgent = GetComponent<NavMeshAgent>();

        MoveToCurrentWaypoint();
    }

    private void FollowTrafficLight()
    {
        if (IsCarAhead())
        {
            carAgent.isStopped = true;
            carAgent.speed = 0;
            return;
        }

        Vector3 toStopPoint = stopPoint.position - transform.position;

        bool stopPointAhead = Vector3.Dot(transform.forward, toStopPoint) > 0;
        float distance = toStopPoint.magnitude;

        if (!stopPointAhead)
        {
            carAgent.isStopped = false;
            carAgent.speed = normalSpeed;
            return;
        }

        if (!trafficLight.IsGreen(myDirection) && distance <= stopDistance)
        {
            carAgent.isStopped = true;
            carAgent.speed = 0;
        }
        else if (!trafficLight.IsGreen(myDirection) && distance <= stopDistance * 2f)
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

    private bool IsCarAhead()
    {
        Vector3 origin = transform.TransformPoint(sensorOffset);

        Collider[] hits = Physics.OverlapSphere(
            origin + transform.forward * carDetectionDistance,
            carDetectionRadius,
            carLayer);

        foreach (Collider hit in hits)
        {
            if (hit.transform == transform)
                continue;

            return true;
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
            Destroy(gameObject);
            return;
        }

        MoveToCurrentWaypoint();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 origin = transform.TransformPoint(sensorOffset);

        Gizmos.DrawWireSphere(origin, carDetectionRadius);

        Gizmos.DrawLine(origin,
            origin + transform.forward * carDetectionDistance);

        Gizmos.DrawWireSphere(
            origin + transform.forward * carDetectionDistance,
            carDetectionRadius);
    }
}