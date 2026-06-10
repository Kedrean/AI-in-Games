using UnityEngine;
using UnityEngine.AI;

public class NeutralUnit : MonoBehaviour
{
    public LeaderBase leader;

    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (leader == null)
            return;

        int index = leader.followers.IndexOf(this);

        Vector3 offset =
            new Vector3((index % 5) * 1.5f, 0, (index / 5) * 1.5f);

        agent.SetDestination(leader.transform.position - offset);
    }

    public void SetLeader(LeaderBase newLeader)
    {
        leader = newLeader;

        Renderer r = GetComponent<Renderer>();

        if (r != null)
        {
            r.material.color = newLeader.teamColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (leader != null) 
            return;

        LeaderBase collector = other.GetComponent<LeaderBase>();

        if (collector == null)
            return;

        collector.AddFollower(this);
    }
}
