using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MobileActionButton : MonoBehaviour, IPointerDownHandler
{
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

    private void Awake()
    {
        Instance = this;

        player = FindAnyObjectByType<LyraController>();

        SetJumpMode();
        ApplyMainHallLayout();
    }

    private void ApplyMainHallLayout()
    {
        if (SceneManager.GetActiveScene().name != "MainHall")
            return;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            return;

        RectTransform canvasRect = canvas.transform as RectTransform;
        if (canvasRect != null)
            canvasRect.localScale = Vector3.one;

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
        buttonRect.sizeDelta = new Vector2(160f, 160f);
        buttonRect.anchoredPosition = new Vector2(-24f, 24f);
        buttonRect.localScale = Vector3.one;
        buttonRect.SetAsLastSibling();
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