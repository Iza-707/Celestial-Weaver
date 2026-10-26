using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingSceneController : MonoBehaviour
{
    [Header("Ending Background")]
    [SerializeField] private Sprite endingBackground;

    [Header("Return Button")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private Vector2 buttonAnchor = new Vector2(0.17f, 0.16f);
    [SerializeField] private Vector2 buttonSize = new Vector2(290f, 72f);

    private static Sprite buttonBackgroundSprite;

    private void Start()
    {
        Time.timeScale = 1f;

        // Guard the scene itself as well as the Main Hall star.
        if (GameManager.Instance == null ||
            !GameManager.Instance.HaveAllCatalogueConstellationsBeenCompletedAndRestored())
        {
            Debug.LogWarning("The ending scene is locked until all nine catalogue constellations have been restored and all catalogue levels completed.");
            SceneManager.LoadScene(mainMenuScene);
            return;
        }

        EnsureEventSystem();
        BuildEndingUI();
    }

    private void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystemObject = new GameObject(
            "EventSystem",
            typeof(EventSystem),
            typeof(InputSystemUIInputModule)
        );

        // Adding the UI module assigns its default UI actions in the Input System.
        InputSystemUIInputModule inputModule =
            eventSystemObject.GetComponent<InputSystemUIInputModule>();

        if (inputModule.actionsAsset == null)
            inputModule.AssignDefaultActions();
    }

    private void BuildEndingUI()
    {
        GameObject canvasObject = new GameObject(
            "Ending Canvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        GameObject backgroundObject = new GameObject(
            "Absolutely Stellar Artwork",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(AspectRatioFitter)
        );
        backgroundObject.transform.SetParent(canvasObject.transform, false);

        RectTransform backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        Image backgroundImage = backgroundObject.GetComponent<Image>();
        backgroundImage.sprite = endingBackground;
        backgroundImage.preserveAspect = true;
        backgroundImage.raycastTarget = false;

        AspectRatioFitter aspectFitter =
            backgroundObject.GetComponent<AspectRatioFitter>();
        aspectFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        aspectFitter.aspectRatio = endingBackground != null
            ? endingBackground.rect.width / endingBackground.rect.height
            : 1126f / 697f;

        if (endingBackground == null)
            Debug.LogError("EndingSceneController: assign Absolutely_Stellar.jpg to Ending Background in the Inspector.");

        CreateReturnButton(canvasObject.transform);
    }

    private void CreateReturnButton(Transform parent)
    {
        GameObject buttonObject = new GameObject(
            "Return To Main Menu Button",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button)
        );
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = buttonAnchor;
        rect.anchorMax = buttonAnchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = buttonSize;

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.sprite = GetButtonBackgroundSprite();
        buttonImage.type = Image.Type.Sliced;
        buttonImage.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;
        button.transition = Selectable.Transition.ColorTint;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.95f, 0.78f, 1f);
        colors.pressedColor = new Color(0.70f, 0.63f, 0.88f, 1f);
        colors.selectedColor = new Color(1f, 0.95f, 0.78f, 1f);
        colors.disabledColor = new Color(0.45f, 0.43f, 0.52f, 0.55f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.12f;
        button.colors = colors;
        button.onClick.AddListener(ReturnToMainMenu);

        GameObject labelObject = new GameObject(
            "Button Label",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Text)
        );
        labelObject.transform.SetParent(buttonObject.transform, false);

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(10f, 4f);
        labelRect.offsetMax = new Vector2(-10f, -4f);

        Text label = labelObject.GetComponent<Text>();
        Font builtInFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (builtInFont == null)
            builtInFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        label.font = builtInFont;
        label.text = "RETURN TO MAIN MENU";
        label.fontSize = 23;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = new Color(1f, 0.91f, 0.68f, 1f);
        label.resizeTextForBestFit = true;
        label.resizeTextMinSize = 14;
        label.resizeTextMaxSize = 23;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Overflow;
        label.raycastTarget = false;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(buttonObject);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    private static Sprite GetButtonBackgroundSprite()
    {
        if (buttonBackgroundSprite != null)
            return buttonBackgroundSprite;

        const int width = 64;
        const int height = 32;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.name = "Celestial Ending Button";
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int edge = Mathf.Min(x, y, width - 1 - x, height - 1 - y);
                Color color;

                if (edge < 2)
                    color = new Color(0.94f, 0.72f, 0.35f, 0.98f);
                else if (edge < 4)
                    color = new Color(0.28f, 0.19f, 0.44f, 0.98f);
                else
                {
                    float gradient = (float)y / (height - 1);
                    color = Color.Lerp(
                        new Color(0.045f, 0.035f, 0.12f, 0.96f),
                        new Color(0.12f, 0.075f, 0.22f, 0.96f),
                        gradient
                    );
                }

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply(false, true);
        buttonBackgroundSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, width, height),
            new Vector2(0.5f, 0.5f),
            100f,
            0,
            SpriteMeshType.FullRect,
            new Vector4(6f, 6f, 6f, 6f)
        );
        buttonBackgroundSprite.name = "Celestial Ending Button";
        return buttonBackgroundSprite;
    }
}
