using UnityEngine;
using UnityEngine.AI;

public class CrowdSpawner : MonoBehaviour
{
    public GameObject neutralPrefab;

    public int amount = 100;

    public float spawnRadius = 50f;

    private void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnNeutral();
        }
    }

    void SpawnNeutral()
    {
        Vector3 random = Random.insideUnitSphere * spawnRadius;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(random, out hit, spawnRadius, NavMesh.AllAreas))
        {
            Instantiate(neutralPrefab, hit.position, Quaternion.identity);
        }
    }
}
