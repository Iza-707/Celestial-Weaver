using UnityEngine;

public class StarPenPickup : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "[E] Pick Up Star-Pen";

    private void Start()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.starPenUnlocked)
        {
            gameObject.SetActive(false);
        }
    }

    public void Interact()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.starPenUnlocked = true;
        GameManager.Instance.SaveProgress();

        StarPenController starPen = FindAnyObjectByType<StarPenController>();
        if (starPen != null)
            starPen.ActivateFromPickup();

        Debug.Log("Star-Pen unlocked!");

        gameObject.SetActive(false);
    }
}