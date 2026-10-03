using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("New Game")]
    [SerializeField] private string newGameScene = "Stage0_1";

    [Header("Continue")]
    [SerializeField] private string defaultContinueScene = "Stage0_1";

    public void NewGame()
    {
        PlayerPrefs.DeleteKey("LastGameplayScene");
        PlayerPrefs.Save();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.starPenUnlocked = false;
        }

        //change this to cinematic scene later if we have one
        SceneManager.LoadScene(newGameScene);
    }

    public void ContinueGame()
    {
        string lastScene =
            PlayerPrefs.GetString(
                "LastGameplayScene",
                defaultContinueScene
            );

        SceneManager.LoadScene(lastScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}