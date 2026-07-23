using UnityEngine;

public class StopLight : MonoBehaviour
{
    [Header("Light Objects")]
    public GameObject northLight;
    public GameObject eastLight;
    public GameObject southLight;
    public GameObject westLight;

    [Header("Timing")]
    public float greenTime = 8f;
    public float yellowTime = 2f;

    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    private enum Phase
    {
        NorthSouthGreen,
        NorthSouthYellow,
        EastWestGreen,
        EastWestYellow
    }

    private Phase currentPhase = Phase.NorthSouthGreen;
    private float timer;

    public bool IsGreen(Direction direction)
    {
        switch (currentPhase)
        {
            case Phase.NorthSouthGreen:
                return direction == Direction.North || direction == Direction.South;

            case Phase.EastWestGreen:
                return direction == Direction.East || direction == Direction.West;

            default:
                return false;
        }
    }

    void Start()
    {
        UpdateLights();
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (currentPhase)
        {
            case Phase.NorthSouthGreen:

                if (timer >= greenTime)
                {
                    timer = 0;
                    currentPhase = Phase.NorthSouthYellow;
                    UpdateLights();
                }

                break;

            case Phase.NorthSouthYellow:

                if (timer >= yellowTime)
                {
                    timer = 0;
                    currentPhase = Phase.EastWestGreen;
                    UpdateLights();
                }

                break;

            case Phase.EastWestGreen:

                if (timer >= greenTime)
                {
                    timer = 0;
                    currentPhase = Phase.EastWestYellow;
                    UpdateLights();
                }

                break;

            case Phase.EastWestYellow:

                if (timer >= yellowTime)
                {
                    timer = 0;
                    currentPhase = Phase.NorthSouthGreen;
                    UpdateLights();
                }

                break;
        }
    }

    void UpdateLights()
    {
        bool nsGreen = currentPhase == Phase.NorthSouthGreen;
        bool ewGreen = currentPhase == Phase.EastWestGreen;

        northLight.SetActive(nsGreen);
        southLight.SetActive(nsGreen);

        eastLight.SetActive(ewGreen);
        westLight.SetActive(ewGreen);
    }
}