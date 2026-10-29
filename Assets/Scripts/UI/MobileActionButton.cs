using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileActionButton : MonoBehaviour, IPointerDownHandler
{
    private const float StandardButtonSize = 120f;

    public static MobileActionButton Instance;

    [Header("Button")]
    public Button button;
    public Image icon;

    [Header("Icons")]
    public Sprite jumpIcon;
    public Sprite interactIcon;

    private IInteractable currentInteractable;
    private LyraController player;
    private bool pressedOnPointerDown;
    private Rect lastSafeArea;

    private void Awake()
    {
        Instance = this;

        player = FindAnyObjectByType<LyraController>();

        SetJumpMode();
        ApplyStandardLayout();
    }

    private void ApplyStandardLayout()
    {
        lastSafeArea = Screen.safeArea;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        RectTransform buttonRect = transform as RectTransform;
        if (buttonRect == null)
            return;

        buttonRect.SetParent(canvas.transform, false);
        buttonRect.anchorMin = new Vector2(
            Screen.safeArea.xMax / Screen.width,
            Screen.safeArea.yMin / Screen.height
        );
        buttonRect.anchorMax = buttonRect.anchorMin;
        buttonRect.pivot = new Vector2(1f, 0f);
        buttonRect.sizeDelta = new Vector2(
            StandardButtonSize,
            StandardButtonSize
        );
        buttonRect.anchoredPosition = new Vector2(-24f, 24f);
        buttonRect.localScale = Vector3.one;
        buttonRect.SetAsLastSibling();

        if (icon != null)
            icon.preserveAspect = true;
    }

    private void Update()
    {
        if (Screen.safeArea != lastSafeArea)
            ApplyStandardLayout();
    }

    public void SetInteractable(IInteractable interactable)
    {
        currentInteractable = interactable;

        icon.sprite = interactIcon;
    }

    public void SetJumpMode()
    {
        currentInteractable = null;

        icon.sprite = jumpIcon;
    }

    public void Press()
    {
        if (pressedOnPointerDown)
        {
            pressedOnPointerDown = false;
            return;
        }

        PerformAction();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressedOnPointerDown = true;
        PerformAction();
    }

    private void PerformAction()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
            return;
        }

        if (player != null)
        {
            player.MobileJump();
        }
    }
}