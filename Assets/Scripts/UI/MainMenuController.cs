using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        if(SceneScript.Instance != null) {
            SceneScript.Instance.NextScene("Stage0_1");
        } else {
            SceneManager.LoadScene("Stage0_1");
        }
    }

    public void ContinueGame()
    {
        if(SceneScript.Instance != null) {
            SceneScript.Instance.NextScene("Stage0_1");
        } else {
        SceneManager.LoadScene("Stage0_1");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}