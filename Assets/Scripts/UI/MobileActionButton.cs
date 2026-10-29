using UnityEngine;
using UnityEngine.EventSystems;
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