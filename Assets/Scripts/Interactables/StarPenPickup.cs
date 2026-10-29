using UnityEngine;

public class StarPenPickup : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "[E] Pick Up Star-Pen";

    public void Interact()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.starPenUnlocked = true;

        StarPenController starPen = FindAnyObjectByType<StarPenController>();
        if (starPen != null)
            starPen.ActivateFromPickup();

        Debug.Log("Star-Pen unlocked!");

        gameObject.SetActive(false);
    }
}