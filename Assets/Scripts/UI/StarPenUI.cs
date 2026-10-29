using UnityEngine;
using UnityEngine.UI;

public class StarPenUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject starPenIndicator_Draw;
    public GameObject starPenIndicator_Erase;
    public Slider lightEnergyGauge;

    private StarPenController starPenController;
    private CanvasGroup tracingCanvasGroup;
    private float previousCanvasAlpha;
    private bool previousCanvasInteractable;
    private bool previousCanvasBlocksRaycasts;
    private bool isTracingUIHidden;
    private Rect lastSafeArea;

    void Start()
    {
        starPenController = FindAnyObjectByType<StarPenController>();
        ApplyStandardGameplayLayout();
        MakeEnergyGaugeNonInteractive();
        ConfigureDesktopButtons();

        UpdateUI();
    }

    private void ApplyStandardGameplayLayout()
    {
        lastSafeArea = Screen.safeArea;

        Vector2 safeAreaMin = new Vector2(
            lastSafeArea.xMin / Screen.width,
            lastSafeArea.yMin / Screen.height
        );
        Vector2 safeAreaMax = new Vector2(
            lastSafeArea.xMax / Screen.width,
            lastSafeArea.yMax / Screen.height
        );
        float safeAreaCenterX =
            (safeAreaMin.x + safeAreaMax.x) * 0.5f;

        SetLayout(
            "PauseButton",
            new Vector2(safeAreaMin.x, safeAreaMax.y),
            new Vector2(0f, 1f),
            new Vector2(16f, -16f),
            new Vector2(800f, 800f),
            new Vector3(0.1f, 0.1f, 0.1f)
        );
        SetLayout(
            "LightEnergyGauge",
            new Vector2(safeAreaCenterX, safeAreaMax.y),
            new Vector2(0.5f, 1f),
            new Vector2(0f, -38f)
        );
        SetLayout(
            "StarPenButton_Draw",
            new Vector2(safeAreaMax.x, safeAreaMax.y),
            Vector2.one,
            new Vector2(-16f, -16f),
            scale: new Vector3(2.6f, 2.6f, 1f)
        );
        SetLayout(
            "StarPenButton_Erase",
            new Vector2(safeAreaMax.x, safeAreaMax.y),
            Vector2.one,
            new Vector2(-16f, -100f),
            scale: new Vector3(2.6f, 2.6f, 1f)
        );
        SetLayout(
            "InteractionPrompt",
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0f, 40f)
        );
        SetLayout(
            "HoldingStarUI",
            new Vector2(safeAreaCenterX, safeAreaMax.y),
            new Vector2(0.5f, 1f),
            new Vector2(0f, -92f)
        );
    }

    private void SetLayout(
        string objectName,
        Vector2 anchor,
        Vector2 pivot,
        Vector2 position,
        Vector2? size = null,
        Vector3? scale = null)
    {
        Transform element = transform.Find(objectName);
        if (element == null)
            return;

        RectTransform rect = element as RectTransform;
        if (rect == null)
            return;

        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = position;

        if (size.HasValue)
            rect.sizeDelta = size.Value;

        if (scale.HasValue)
            rect.localScale = scale.Value;
    }

    private void MakeEnergyGaugeNonInteractive()
    {
        if (lightEnergyGauge == null)
            return;

        Graphic[] gaugeGraphics =
            lightEnergyGauge.GetComponentsInChildren<Graphic>(true);

        foreach (Graphic graphic in gaugeGraphics)
            graphic.raycastTarget = false;
    }

    public void SetTracingUIVisible(bool visible)
    {
        if (tracingCanvasGroup == null)
        {
            tracingCanvasGroup = GetComponent<CanvasGroup>();
            if (tracingCanvasGroup == null)
                tracingCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (!visible)
        {
            if (isTracingUIHidden)
                return;

            previousCanvasAlpha = tracingCanvasGroup.alpha;
            previousCanvasInteractable = tracingCanvasGroup.interactable;
            previousCanvasBlocksRaycasts = tracingCanvasGroup.blocksRaycasts;

            tracingCanvasGroup.alpha = 0f;
            tracingCanvasGroup.interactable = false;
            tracingCanvasGroup.blocksRaycasts = false;
            isTracingUIHidden = true;
            return;
        }

        if (!isTracingUIHidden)
            return;

        tracingCanvasGroup.alpha = previousCanvasAlpha;
        tracingCanvasGroup.interactable = previousCanvasInteractable;
        tracingCanvasGroup.blocksRaycasts = previousCanvasBlocksRaycasts;
        isTracingUIHidden = false;
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
        if (Screen.safeArea != lastSafeArea)
            ApplyStandardGameplayLayout();

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