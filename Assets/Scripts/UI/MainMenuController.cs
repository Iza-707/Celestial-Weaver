using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene("Stage0_1");
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("Stage0_1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}