using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AILeader : MonoBehaviour
{
    NavMeshAgent agent;

    float timer;

    public float detectRadius = 20f;
    public float fleeDistance = 15f;

    public float neutralSearchRadius = 50f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        PickRandomDestination();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < 0.5f)
            return;

        timer = 0f;

        LeaderBase myLeader = GetComponent<LeaderBase>();

        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                detectRadius);

        LeaderBase strongestThreat = null;
        LeaderBase weakestTarget = null;

        foreach (Collider hit in hits)
        {
            LeaderBase leader =
                hit.GetComponent<LeaderBase>();

            if (leader == null)
                continue;

            if (leader == myLeader)
                continue;

            if (myLeader.FollowerCount >
                leader.FollowerCount + 3)
            {
                weakestTarget = leader;
            }

            else if
                (leader.FollowerCount >
                 myLeader.FollowerCount + 3)
            {
                strongestThreat = leader;
            }
        }

        if (strongestThreat != null)
        {
            Vector3 dir =
                (transform.position -
                 strongestThreat.transform.position).normalized;

            Vector3 fleePos =
                transform.position +
                dir * fleeDistance;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                fleePos,
                out hit,
                fleeDistance,
                NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            return;
        }

        if (weakestTarget != null)
        {
            agent.SetDestination(
                weakestTarget.transform.position);

            return;
        }

        NeutralUnit nearestNeutral =
            FindNearestNeutral();

        if (nearestNeutral != null)
        {
            agent.SetDestination(
                nearestNeutral.transform.position);

            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance < 2f)
        {
            PickRandomDestination();
        }
    }

    NeutralUnit FindNearestNeutral()
    {
        NeutralUnit[] neutrals =
            FindObjectsByType<NeutralUnit>(
                FindObjectsSortMode.None);

        NeutralUnit closest = null;

        float bestDistance =
            Mathf.Infinity;

        foreach (NeutralUnit neutral in neutrals)
        {
            if (neutral.leader != null)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    neutral.transform.position);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                closest = neutral;
            }
        }

        return closest;
    }

    void PickRandomDestination()
    {
        Vector3 random =
            Random.insideUnitSphere * 30f;

        random += transform.position;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            random,
            out hit,
            30f,
            NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}