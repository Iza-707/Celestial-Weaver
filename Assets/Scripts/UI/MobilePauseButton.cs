using UnityEngine;

public class MobilePauseButton : MonoBehaviour
{
    public GameObject pauseMenu;

    public void TogglePause()
    {
        bool isPaused = Time.timeScale == 0f;

        if (isPaused)
        {
            Time.timeScale = 1f;

            if (pauseMenu != null)
                pauseMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0f;

            if (pauseMenu != null)
                pauseMenu.SetActive(true);
        }
    }
}