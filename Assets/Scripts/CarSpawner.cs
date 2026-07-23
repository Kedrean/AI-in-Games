using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject carPrefab;

    public Transform[] spawnPoints;

    [Header("Routes")]
    public Transform[] northRoutes;
    public Transform[] southRoutes;
    public Transform[] eastRoutes;
    public Transform[] westRoutes;

    [Header("Traffic")]
    public StopLight[] trafficLights;
    public Transform[] stopPoints;

    [Header("Timing")]
    public float spawnInterval = 3f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCar), 0f, spawnInterval);
    }

    void SpawnCar()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);

        Transform spawn = spawnPoints[spawnIndex];

        Transform[] routes = null;

        switch (spawnIndex)
        {
            case 0:
                routes = northRoutes;
                break;

            case 1:
                routes = southRoutes;
                break;

            case 2:
                routes = eastRoutes;
                break;

            case 3:
                routes = westRoutes;
                break;
        }

        if (routes == null || routes.Length == 0)
            return;

        Transform chosenRoute = routes[Random.Range(0, routes.Length)];

        GameObject car = Instantiate(
            carPrefab,
            spawn.position,
            spawn.rotation);

        CarAI ai = car.GetComponent<CarAI>();

        ai.SetRoute(chosenRoute);

        ai.trafficLight = trafficLights[spawnIndex];
        ai.stopPoint = stopPoints[spawnIndex];
    }
}