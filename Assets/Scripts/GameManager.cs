using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const string PlayerDataKey = "PlayerData";
    private const string LastGameplaySceneKey = "LastGameplayScene";

    public static GameManager Instance { get; private set; }

    public bool starPenUnlocked = false;
    [Header("Game Progress")]
    public bool hasReachedMainHall = false;
    private Vector3 currentRespawnPosition;
    private bool hasCheckpoint = false;
    private string checkpointScene;

    [Header("Light Energy Progression")]
    public float maxLightEnergy = StartingMaxLightEnergy;
    public float constellationEnergyIncrease = 10f;

    public const float StartingMaxLightEnergy = 30f;

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

    [System.Serializable]
    private class PlayerData
    {
        public bool starPenUnlocked;
        public bool hasReachedMainHall;
        public float maxLightEnergy;
        public bool cancerRestored;
        public bool taurusRestored;
        public bool scorpioRestored;
        public bool ursaMinorRestored;
        public bool ursaMajorRestored;
        public bool orionRestored;
        public bool cassiopeiaRestored;
        public bool cepheusRestored;
        public bool CygnusRestored;
        public bool cancerCompleted;
        public bool taurusCompleted;
        public bool scorpioCompleted;
        public bool ursaMinorCompleted;
        public bool ursaMajorCompleted;
        public bool orionCompleted;
        public bool cassiopeiaCompleted;
        public bool cepheusCompleted;
        public bool CygnusCompleted;
        public bool hasCheckpoint;
        public string checkpointScene;
        public float checkpointX;
        public float checkpointY;
        public float checkpointZ;
    }

    /// <summary>
    /// True only after all nine catalogue constellations have been restored
    /// and their levels have been completed by reaching the exits.
    /// </summary>
    public bool HaveAllCatalogueConstellationsBeenCompletedAndRestored()
    {
        return
            cancerCompleted && cancerRestored &&
            taurusCompleted && taurusRestored &&
            scorpioCompleted && scorpioRestored &&
            ursaMinorCompleted && ursaMinorRestored &&
            ursaMajorCompleted && ursaMajorRestored &&
            orionCompleted && orionRestored &&
            cassiopeiaCompleted && cassiopeiaRestored &&
            cepheusCompleted && cepheusRestored &&
            CygnusCompleted && CygnusRestored;
    }

    public void IncreaseMaxLightEnergy()
    {
        maxLightEnergy += constellationEnergyIncrease;
        SaveProgress();
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
        LoadProgress();
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        if (checkpoint == null)
            return;

        currentRespawnPosition = checkpoint.position;
        hasCheckpoint = true;
        checkpointScene = SceneManager.GetActiveScene().name;
        SaveProgress();
        Debug.Log("Checkpoint saved at " + currentRespawnPosition);
    }

    public bool ApplySavedCheckpoint(Transform player)
    {
        if (!hasCheckpoint ||
            player == null ||
            checkpointScene != SceneManager.GetActiveScene().name)
        {
            return false;
        }

        player.position = currentRespawnPosition;
        return true;
    }

    public void ClearCheckpoint()
    {
        hasCheckpoint = false;
        checkpointScene = null;
        currentRespawnPosition = Vector3.zero;
        SaveProgress();
    }

    public void MarkMainHallReached()
    {
        hasReachedMainHall = true;
        PlayerPrefs.SetString(LastGameplaySceneKey, "MainHall");
        SaveProgress();
    }

    public void SaveGameplayScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName) || sceneName == "MainMenu")
            return;

        PlayerPrefs.SetString(LastGameplaySceneKey, sceneName);

        if (hasCheckpoint && checkpointScene != sceneName)
        {
            hasCheckpoint = false;
            checkpointScene = null;
        }

        SaveProgress();
    }

    public void ResetProgress()
    {
        starPenUnlocked = false;
        hasReachedMainHall = false;
        maxLightEnergy = StartingMaxLightEnergy;

        cancerRestored = false;
        taurusRestored = false;
        scorpioRestored = false;
        ursaMinorRestored = false;
        ursaMajorRestored = false;
        orionRestored = false;
        cassiopeiaRestored = false;
        cepheusRestored = false;
        CygnusRestored = false;

        cancerCompleted = false;
        taurusCompleted = false;
        scorpioCompleted = false;
        ursaMinorCompleted = false;
        ursaMajorCompleted = false;
        orionCompleted = false;
        cassiopeiaCompleted = false;
        cepheusCompleted = false;
        CygnusCompleted = false;

        hasCheckpoint = false;
        checkpointScene = null;
        currentRespawnPosition = Vector3.zero;

        PlayerPrefs.DeleteKey(PlayerDataKey);
        PlayerPrefs.DeleteKey(LastGameplaySceneKey);
        PlayerPrefs.Save();
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

    public void RestoreConstellation(string constellationID)
    {
        switch (constellationID.ToUpper())
        {
            case "CANCER":
                cancerRestored = true;
                break;
            case "TAURUS":
                taurusRestored = true;
                break;
            case "SCORPIO":
                scorpioRestored = true;
                break;
            case "URSA MINOR":
                ursaMinorRestored = true;
                break;
            case "URSA MAJOR":
                ursaMajorRestored = true;
                break;
            case "ORION":
                orionRestored = true;
                break;
            case "CASSIOPEIA":
                cassiopeiaRestored = true;
                break;
            case "CEPHEUS":
                cepheusRestored = true;
                break;
            case "CYGNUS":
                CygnusRestored = true;
                break;
            default:
                Debug.LogWarning("Unknown constellation: " + constellationID);
                return;
        }

        SaveProgress();
        Debug.Log(constellationID + " restored!");
    }

    public void CompleteLevel(string constellationID)
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
            case "URSA MINOR":
                ursaMinorCompleted = true;
                break;
            case "URSA MAJOR":
                ursaMajorCompleted = true;
                break;
            case "ORION":
                orionCompleted = true;
                break;
            case "CASSIOPEIA":
                cassiopeiaCompleted = true;
                break;
            case "CEPHEUS":
                cepheusCompleted = true;
                break;
            case "CYGNUS":
                CygnusCompleted = true;
                break;
            default:
                Debug.LogWarning("Unknown level: " + constellationID);
                return;
        }

        SaveProgress();
        Debug.Log(constellationID + " level completed!");
    }

    public void SaveProgress()
    {
        PlayerData data = new PlayerData
        {
            starPenUnlocked = starPenUnlocked,
            hasReachedMainHall = hasReachedMainHall,
            maxLightEnergy = maxLightEnergy,
            cancerRestored = cancerRestored,
            taurusRestored = taurusRestored,
            scorpioRestored = scorpioRestored,
            ursaMinorRestored = ursaMinorRestored,
            ursaMajorRestored = ursaMajorRestored,
            orionRestored = orionRestored,
            cassiopeiaRestored = cassiopeiaRestored,
            cepheusRestored = cepheusRestored,
            CygnusRestored = CygnusRestored,
            cancerCompleted = cancerCompleted,
            taurusCompleted = taurusCompleted,
            scorpioCompleted = scorpioCompleted,
            ursaMinorCompleted = ursaMinorCompleted,
            ursaMajorCompleted = ursaMajorCompleted,
            orionCompleted = orionCompleted,
            cassiopeiaCompleted = cassiopeiaCompleted,
            cepheusCompleted = cepheusCompleted,
            CygnusCompleted = CygnusCompleted,
            hasCheckpoint = hasCheckpoint,
            checkpointScene = checkpointScene,
            checkpointX = currentRespawnPosition.x,
            checkpointY = currentRespawnPosition.y,
            checkpointZ = currentRespawnPosition.z
        };

        PlayerPrefs.SetString(PlayerDataKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        if (!PlayerPrefs.HasKey(PlayerDataKey))
            return;

        string json = PlayerPrefs.GetString(PlayerDataKey);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Saved player data is empty.");
            return;
        }

        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        if (data == null)
        {
            Debug.LogError("Could not load saved player data.");
            return;
        }

        starPenUnlocked = data.starPenUnlocked;
        hasReachedMainHall = data.hasReachedMainHall;
        maxLightEnergy = data.maxLightEnergy > 0f
            ? data.maxLightEnergy
            : StartingMaxLightEnergy;
        cancerRestored = data.cancerRestored;
        taurusRestored = data.taurusRestored;
        scorpioRestored = data.scorpioRestored;
        ursaMinorRestored = data.ursaMinorRestored;
        ursaMajorRestored = data.ursaMajorRestored;
        orionRestored = data.orionRestored;
        cassiopeiaRestored = data.cassiopeiaRestored;
        cepheusRestored = data.cepheusRestored;
        CygnusRestored = data.CygnusRestored;
        cancerCompleted = data.cancerCompleted;
        taurusCompleted = data.taurusCompleted;
        scorpioCompleted = data.scorpioCompleted;
        ursaMinorCompleted = data.ursaMinorCompleted;
        ursaMajorCompleted = data.ursaMajorCompleted;
        orionCompleted = data.orionCompleted;
        cassiopeiaCompleted = data.cassiopeiaCompleted;
        cepheusCompleted = data.cepheusCompleted;
        CygnusCompleted = data.CygnusCompleted;
        hasCheckpoint = data.hasCheckpoint;
        checkpointScene = data.checkpointScene;
        currentRespawnPosition = new Vector3(
            data.checkpointX,
            data.checkpointY,
            data.checkpointZ
        );
    }
}
