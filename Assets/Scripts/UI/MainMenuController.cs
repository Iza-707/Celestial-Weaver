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
        PlayerPrefs.DeleteKey("LastGameplayScene");
        PlayerPrefs.Save();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.starPenUnlocked = false;
            GameManager.Instance.hasReachedMainHall = false;

            // Reset constellation progression
            GameManager.Instance.cancerRestored = false;
            GameManager.Instance.taurusRestored = false;
            GameManager.Instance.scorpioRestored = false;

            GameManager.Instance.ursaMinorRestored = false;
            GameManager.Instance.ursaMajorRestored = false;
            GameManager.Instance.orionRestored = false;

            GameManager.Instance.cassiopeiaRestored = false;
            GameManager.Instance.cepheusRestored = false;
            GameManager.Instance.CygnusRestored = false;

            GameManager.Instance.cancerCompleted = false;
            GameManager.Instance.taurusCompleted = false;
            GameManager.Instance.scorpioCompleted = false;

            GameManager.Instance.ursaMinorCompleted = false;
            GameManager.Instance.ursaMajorCompleted = false;
            GameManager.Instance.orionCompleted = false;

            GameManager.Instance.cassiopeiaCompleted = false;
            GameManager.Instance.cepheusCompleted = false;
            GameManager.Instance.CygnusCompleted = false;
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