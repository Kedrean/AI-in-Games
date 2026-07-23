using UnityEngine;

public class PedestrianSpawner : MonoBehaviour
{
    public GameObject pedestrianPrefab;

    public Transform spawnPoint;

    public float spawnInterval = 8f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnPedestrian),
                        0f,
                        spawnInterval);
    }

    void SpawnPedestrian()
    {
        Instantiate(
            pedestrianPrefab,
            spawnPoint.position,
            spawnPoint.rotation);
    }
}