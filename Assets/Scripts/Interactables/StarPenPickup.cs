using UnityEngine;

public class StarPenPickup : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "[E] Pick Up Star-Pen";

    public void Interact()
    {
        GameManager.Instance.starPenUnlocked = true;

        Debug.Log("Star-Pen unlocked!");

        gameObject.SetActive(false);
    }
}