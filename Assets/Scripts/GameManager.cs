using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool starPenUnlocked = false;
    [Header("Game Progress")]
    public bool hasReachedMainHall = false;
    private Vector3 currentRespawnPosition;
    private bool hasCheckpoint = false;

    [Header("Light Energy Progression")]
    public float maxLightEnergy = 30f;
    public float constellationEnergyIncrease = 10f;

    [Header("Constellation Progress")]
    public bool cancerRestored = false;
    public bool taurusRestored = false;
    public bool scorpioRestored = false;
    public bool ursaMinorRestored = false;
    public bool ursaMajorRestored = false;
    public bool orionRestored = false;
    public bool cassiopeiaRestored = false;
    public bool cepheusRestored = false;
    public bool CygnusRestored = false;

    [Header("Zodiac Progression")]
    public bool cancerCompleted = false;
    public bool taurusCompleted = false;
    public bool scorpioCompleted = false;

    [Header("Navigation and Orientation Progression")]
    public bool ursaMinorCompleted = false;
    public bool ursaMajorCompleted = false;
    public bool orionCompleted = false;

    [Header("Legends and Myths Progression")]
    public bool cassiopeiaCompleted = false;
    public bool cepheusCompleted = false;
    public bool CygnusCompleted = false;

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

    public bool IsLevelUnlocked(string constellationID)
    {
        switch (constellationID.ToUpper())
        {
            case "CANCER":
                return true;

            case "TAURUS":
                return cancerCompleted;

            case "SCORPIO":
                return taurusCompleted;

            default:
                return false;
        }
    }

    public void CompleteConstellation(string constellationID)
    {
        switch (constellationID.ToUpper())
        {
            case "CANCER":
                cancerCompleted = true;
                break;

            case "TAURUS":
                taurusCompleted = true;
                break;

            case "SCORPIO":
                scorpioCompleted = true;
                break;
        }

        Debug.Log(constellationID + " completed!");
    }
}