using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteractionController : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionRange = 1.2f;
    public LayerMask interactableLayer;

    [Header("Prompt")]
    public TextMeshProUGUI interactionPrompt;

    private IInteractable currentInteractable;

    void Update()
    {
        DetectInteractable();

        if (currentInteractable != null)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                currentInteractable.Interact();
            }
        }
    }

    void DetectInteractable()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            interactionRange,
            interactableLayer
        );

        IInteractable closestInteractable = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();

            if (interactable != null)
            {
                float distance = Vector2.Distance(
                    transform.position,
                    hit.transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        currentInteractable = closestInteractable;

        // Mobile action button
        if (MobileActionButton.Instance != null)
        {
            if (currentInteractable != null)
            {
                MobileActionButton.Instance.SetInteractable(
                    currentInteractable
                );
            }
            else
            {
                MobileActionButton.Instance.SetJumpMode();
            }
        }

        UpdatePrompt();
    }

    void UpdatePrompt()
    {
        if (interactionPrompt == null)
            return;

        if (currentInteractable != null)
        {
            interactionPrompt.text = currentInteractable.InteractionPrompt;
            interactionPrompt.gameObject.SetActive(true);
        }
        else
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }
}