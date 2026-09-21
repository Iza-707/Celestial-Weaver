using UnityEngine;
using UnityEngine.UI;

public class MobileActionButton : MonoBehaviour
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