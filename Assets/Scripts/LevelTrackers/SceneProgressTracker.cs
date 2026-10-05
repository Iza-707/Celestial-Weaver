using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneProgressTracker : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        string sceneName = scene.name;

        // Reaching Main Hall permanently unlocks
        // the Main Hall as the Continue hub.
        if (sceneName == "MainHall")
        {
            if (GameManager.Instance != null)
                GameManager.Instance.hasReachedMainHall = true;

            return;
        }

        // Do not save menu scenes as gameplay progress.
        if (sceneName == "MainMenu")
            return;

        // Save ANY gameplay scene.
        PlayerPrefs.SetString(
            "LastGameplayScene",
            sceneName
        );

        PlayerPrefs.Save();

        Debug.Log(
            "Last gameplay scene saved: " +
            sceneName
        );
    }
}