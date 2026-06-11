using UnityEngine;
using UnityEngine.AI;

public class NeutralUnit : MonoBehaviour
{
    public LeaderBase leader;

    NavMeshAgent agent;

    float wanderTimer;
    float followTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (leader == null)
        {
            Wander();

            return;
        }

        FollowLeader();
    }

    void Wander()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer < 1f)
            return;

        wanderTimer = 0f;

        Vector3 random =
            transform.position +
            Random.insideUnitSphere * 8f;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            random,
            out hit,
            8f,
            NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void FollowLeader()
    {
        followTimer += Time.deltaTime;

        if (followTimer < 0.2f)
            return;

        followTimer = 0f;

        int index =
            leader.followers.IndexOf(this);

        Vector3 offset =
            new Vector3(
                (index % 5) * 1.5f,
                0,
                (index / 5) * 1.5f);

        agent.SetDestination(
            leader.transform.position -
            offset);
    }

    public void SetLeader(LeaderBase newLeader)
    {
        leader = newLeader;

        Renderer r =
            GetComponent<Renderer>();

        if (r != null)
        {
            Color followerColor =
                newLeader.teamColor * 0.7f;

            followerColor.a = 1f;

            r.material.color =
                followerColor;
        }
    }
}