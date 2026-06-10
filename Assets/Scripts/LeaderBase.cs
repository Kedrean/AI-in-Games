using UnityEngine;
using System.Collections.Generic;

public class LeaderBase : MonoBehaviour
{
    public Color teamColor;

    [HideInInspector]
    public List<NeutralUnit> followers = new();

    public int FollowerCount => followers.Count;

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

    private void OnCollisionEnter(Collision collision)
    {
        LeaderBase other = collision.gameObject.GetComponent<LeaderBase>();

        if (other == null)
            return;

        if (other == this)
            return;

        if (FollowerCount > other.FollowerCount)
        {
            Destroy(other.gameObject);
        }

        else if (FollowerCount < other.FollowerCount)
        {
            Destroy(gameObject);
        }
    }
}
