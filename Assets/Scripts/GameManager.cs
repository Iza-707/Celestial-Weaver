using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool starPenUnlocked = false;

    private Vector3 currentRespawnPosition;
    private bool hasCheckpoint = false;

    [Header("Light Energy Progression")]
    public float maxLightEnergy = 30f;
    public float constellationEnergyIncrease = 10f;

    public void IncreaseMaxLightEnergy()
    {
        maxLightEnergy += constellationEnergyIncrease;

        Debug.Log("Maximum Light Energy increased to: " + maxLightEnergy);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        currentRespawnPosition = checkpoint.position;
        hasCheckpoint = true;

        Debug.Log("Checkpoint saved at " + currentRespawnPosition);
    }

    public bool HasCheckpoint()
    {
        return hasCheckpoint;
    }

    public Vector3 GetRespawnPosition()
    {
        return currentRespawnPosition;
    }
}