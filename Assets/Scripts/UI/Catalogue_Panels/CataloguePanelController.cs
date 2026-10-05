using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CataloguePanelController : MonoBehaviour
{
    public static CataloguePanelController Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;

    [Header("Level Buttons")]
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private TMP_Text[] levelLabels;

    [Header("Zodiac Scenes")]
    [SerializeField] private string[] zodiacScenes;

    [Header("Navigation Scenes")]
    [SerializeField] private string[] navigationScenes;

    [Header("Legends & Myths Scenes")]
    [SerializeField] private string[] legendsScenes;

    private string currentCatalogue;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    public void OpenCatalogue(string catalogueName)
    {
        if (panel == null)
            return;

        currentCatalogue = catalogueName;

        panel.SetActive(true);

        if (titleText != null)
            titleText.text = catalogueName.ToUpper();

        SetupCatalogue(catalogueName);

        Time.timeScale = 0f;
    }

    private void SetupCatalogue(string catalogueName)
    {
        string[] levels;
        bool[] unlocked;

        switch (catalogueName)
        {
            case "ZODIAC CATALOGUE":
                levels = new string[]
                {
                    "CANCER",
                    "TAURUS",
                    "SCORPIO"
                };

                unlocked = new bool[]
                {
                    true,
                    IsCompleted("CANCER"),
                    IsCompleted("TAURUS")
                };
                break;

            case "NAVIGATION CATALOGUE":
                levels = new string[]
                {
                    "URSA MINOR",
                    "URSA MAJOR",
                    "ORION"
                };

                unlocked = new bool[]
                {
                    true,
                    IsCompleted("URSA MINOR"),
                    IsCompleted("URSA MAJOR")
                };
                break;

            case "LEGENDS & MYTHS":
                levels = new string[]
                {
                    "CASSIOPEIA",
                    "CEPHEUS",
                    "CYGNUS"
                };

                unlocked = new bool[]
                {
                    true,
                    IsCompleted("CASSIOPEIA"),
                    IsCompleted("CEPHEUS")
                };
                break;

            default:
                levels = new string[]
                {
                    "LEVEL 1",
                    "LEVEL 2",
                    "LEVEL 3"
                };

                unlocked = new bool[]
                {
                    true,
                    false,
                    false
                };
                break;
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i >= levels.Length)
            {
                levelButtons[i].gameObject.SetActive(false);
                continue;
            }

            levelButtons[i].gameObject.SetActive(true);

            if (levelLabels != null &&
                i < levelLabels.Length &&
                levelLabels[i] != null)
            {
                levelLabels[i].text = levels[i];
            }

            levelButtons[i].interactable = unlocked[i];
        }
    }

    private bool IsCompleted(string constellationID)
    {
        if (GameManager.Instance == null)
            return false;

        switch (constellationID.ToUpper())
        {
            case "CANCER":
                return GameManager.Instance.cancerCompleted;

            case "TAURUS":
                return GameManager.Instance.taurusCompleted;

            case "SCORPIO":
                return GameManager.Instance.scorpioCompleted;

            case "URSA MINOR":
                return GameManager.Instance.ursaMinorCompleted;

            case "URSA MAJOR":
                return GameManager.Instance.ursaMajorCompleted;

            case "ORION":
                return GameManager.Instance.orionCompleted;

            case "CASSIOPEIA":
                return GameManager.Instance.cassiopeiaCompleted;

            case "CEPHEUS":
                return GameManager.Instance.cepheusCompleted;

            case "CYGNUS":
                return GameManager.Instance.CygnusCompleted;

            default:
                return false;
        }
    }

    public void SelectLevel(int index)
    {
        string[] selectedScenes = null;

        switch (currentCatalogue)
        {
            case "ZODIAC CATALOGUE":
                selectedScenes = zodiacScenes;
                break;

            case "NAVIGATION CATALOGUE":
                selectedScenes = navigationScenes;
                break;

            case "LEGENDS & MYTHS":
                selectedScenes = legendsScenes;
                break;
        }

        if (selectedScenes == null ||
            index < 0 ||
            index >= selectedScenes.Length)
        {
            Debug.LogWarning(
                "Invalid level index for " +
                currentCatalogue +
                ": " +
                index
            );
            return;
        }

        if (string.IsNullOrEmpty(selectedScenes[index]))
        {
            Debug.LogWarning(
                "No scene assigned for level index: " +
                index
            );
            return;
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(selectedScenes[index]);
    }

    public void CloseCatalogue()
    {
        if (panel != null)
            panel.SetActive(false);

        Time.timeScale = 1f;
    }
}