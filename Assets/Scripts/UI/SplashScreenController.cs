using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    [SerializeField] private float splashDuration = 3f;

    private void Start()
    {
        Invoke(nameof(LoadMainMenu), splashDuration);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}