using UnityEngine;
using UnityEngine.UI;

public class StarPenUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject starPenIndicator_Draw;
    public GameObject starPenIndicator_Erase;
    public Slider lightEnergyGauge;

    private StarPenController starPenController;

    void Start()
    {
        starPenController = FindAnyObjectByType<StarPenController>();
        ConfigureDesktopButtons();

        UpdateUI();
    }

    private void ConfigureDesktopButtons()
    {
        if (MobileStarPenControls.Instance != null)
            return;

        Transform[] children = GetComponentsInChildren<Transform>(true);
        Transform drawTransform = null;
        Transform eraseTransform = null;

        foreach (Transform child in children)
        {
            if (child.name == "StarPenToggle_Draw")
                drawTransform = child;
            else if (child.name == "StarPenToggle_Erase")
                eraseTransform = child;
        }

        if (drawTransform == null || eraseTransform == null)
            return;

        starPenIndicator_Draw = drawTransform.gameObject;
        starPenIndicator_Erase = eraseTransform.gameObject;

        Button drawButton = GetOrCreateButton(drawTransform.gameObject);
        Button eraseButton = GetOrCreateButton(eraseTransform.gameObject);

        MobileStarPenControls controls = GetComponent<MobileStarPenControls>();
        if (controls == null)
            controls = gameObject.AddComponent<MobileStarPenControls>();

        controls.ConfigureButtons(drawButton, eraseButton);
    }

    private Button GetOrCreateButton(GameObject buttonObject)
    {
        Button button = buttonObject.GetComponent<Button>();
        if (button != null)
            return button;

        Image image = buttonObject.GetComponent<Image>();
        if (image == null)
            return null;

        button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.None;
        return button;
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (GameManager.Instance == null)
            return;

        bool unlocked = GameManager.Instance.starPenUnlocked;

        if (starPenIndicator_Draw != null)
            starPenIndicator_Draw.SetActive(unlocked);

        if (starPenIndicator_Erase != null)
            starPenIndicator_Erase.SetActive(unlocked);

        if (lightEnergyGauge != null)
        {
            lightEnergyGauge.gameObject.SetActive(unlocked);

            if (starPenController != null)
            {
                lightEnergyGauge.maxValue =
                    starPenController.MaxEnergyValue;

                lightEnergyGauge.value =
                    starPenController.CurrentEnergy;
            }
        }
    }
}