using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AILeader : MonoBehaviour
{
    NavMeshAgent agent;

    float timer;

    public float detectRadius = 20f;
    public float fleeDistance = 15f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        PickRandomDestination();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        LeaderBase myLeader = GetComponent<LeaderBase>();

        Collider[] hits =
            Physics.OverlapSphere(transform.position, detectRadius);

        LeaderBase strongestThreat = null;
        LeaderBase weakestTarget = null;

        foreach (Collider hit in hits)
        {
            LeaderBase leader = hit.GetComponent<LeaderBase>();

            if (leader == null)
                continue;

            if (leader == myLeader)
                continue;

            if (myLeader.FollowerCount > leader.FollowerCount + 3)
            {
                weakestTarget = leader;
            }

            else if (leader.FollowerCount > myLeader.FollowerCount + 3)
            {
                strongestThreat = leader;
            }
        }

        if (strongestThreat != null)
        {
            Vector3 dir = (transform.position - strongestThreat.transform.position).normalized;

            Vector3 fleePos = transform.position + dir * fleeDistance;

            agent.SetDestination(fleePos);

            return;
        }

        if (weakestTarget != null)
        {
            agent.SetDestination(weakestTarget.transform.position);

            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 2f)
        {
            PickRandomDestination();
        }
    }

    void PickRandomDestination()
    {
        Vector3 random = Random.insideUnitSphere * 30f;

        random += transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(random, out hit, 30f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
