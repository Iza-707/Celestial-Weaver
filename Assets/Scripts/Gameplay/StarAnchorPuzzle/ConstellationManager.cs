using UnityEngine;

public class ConstellationManager : MonoBehaviour
{
    public static ConstellationManager Instance { get; private set; }

    [Header("Constellation")]
    [SerializeField] private int requiredStars = 4;

    private int placedStars = 0;

    public bool IsComplete => placedStars >= requiredStars;
    public int PlacedStars => placedStars;
    public int RequiredStars => requiredStars;

    private void Awake()
    {
        Instance = this;
    }

    public void StarPlaced()
    {
        placedStars++;

        Debug.Log(
            "Constellation progress: " +
            placedStars + "/" + requiredStars
        );
    }
}