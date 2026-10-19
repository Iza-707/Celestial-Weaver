
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBGM : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundMusic;

    private void Start()
    {
        Debug.Log(
            $"SceneBGM started in scene: {SceneManager.GetActiveScene().name}, " +
            $"Object: {gameObject.name}, " +
            $"Clip: {(backgroundMusic != null ? backgroundMusic.name : "NONE")}"
        );

        if (AudioManager.Instance != null && backgroundMusic != null)
        {
            AudioManager.Instance.PlayMusic(backgroundMusic);
        }
        else
        {
            Debug.LogError("AudioManager or Background Music is missing!");
        }
    }
}
