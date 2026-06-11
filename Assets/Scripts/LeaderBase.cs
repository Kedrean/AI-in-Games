using UnityEngine;
using System.Collections.Generic;

public class LeaderBase : MonoBehaviour
{
    public Color teamColor;

    public float collectRadius = 2f;
    public float combatRadius = 1.5f;

    [HideInInspector]
    public List<NeutralUnit> followers = new();

    public int FollowerCount => followers.Count;

    private void Update()
    {
        CollectNearbyNeutrals();

        CheckLeaderCombat();
    }

    public void AddFollower(NeutralUnit unit)
    {
        if (followers.Contains(unit))
            return;

        followers.Add(unit);

        unit.SetLeader(this);
    }

    public void RemoveFollower(NeutralUnit unit)
    {
        followers.Remove(unit);
    }

    void CollectNearbyNeutrals()
    {
        Collider[] hits =
            Physics.OverlapSphere(
                transform.position,
                collectRadius);

        foreach (Collider hit in hits)
        {
            NeutralUnit neutral =
                hit.GetComponent<NeutralUnit>();

            if (neutral == null)
                continue;

            if (neutral.leader != null)
                continue;

            AddFollower(neutral);
        }
    }

    void CheckLeaderCombat()
    {
        LeaderBase[] leaders =
            FindObjectsByType<LeaderBase>(
                FindObjectsSortMode.None);

        foreach (LeaderBase other in leaders)
        {
            if (other == this)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    other.transform.position);

            if (distance > combatRadius)
                continue;

            if (FollowerCount >
                other.FollowerCount)
            {
                Destroy(other.gameObject);
            }

            else if
                (FollowerCount <
                 other.FollowerCount)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (NeutralUnit follower in followers)
        {
            if (follower != null)
            {
                follower.leader = null;
            }
        }
    }
}