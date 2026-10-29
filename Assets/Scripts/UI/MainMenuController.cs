using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("New Game")]
    [SerializeField] private string newGameScene = "Stage0_1";

    [Header("Continue")]
    [SerializeField] private string defaultContinueScene = "Stage0_1";
    [SerializeField] private string mainHallScene = "MainHall";

    public void NewGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetProgress();
        }
        else
        {
            PlayerPrefs.DeleteKey("PlayerData");
            PlayerPrefs.DeleteKey("LastGameplayScene");
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene(newGameScene);
    }

    public void ContinueGame()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.hasReachedMainHall)
        {
            SceneManager.LoadScene(mainHallScene);
            return;
        }

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