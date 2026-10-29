using System.Collections.Generic;
using UnityEngine;

public class ConstellationManager : MonoBehaviour
{
    public static ConstellationManager Instance { get; private set; }

    [Header("Constellation")]
    [SerializeField] private int requiredStars = 4;

    private int placedStars = 0;
    private readonly HashSet<StarAnchor> placedAnchors = new HashSet<StarAnchor>();

    public bool IsComplete => placedStars >= requiredStars;
    public int PlacedStars => placedStars;
    public int RequiredStars => requiredStars;

    private void Awake()
    {
        Instance = this;
    }

    public void StarPlaced(StarAnchor anchor)
    {
        if (anchor == null || !placedAnchors.Add(anchor))
            return;

        placedStars++;

        Debug.Log(
            "Constellation progress: " +
            placedStars + "/" + requiredStars
        );
    }
}