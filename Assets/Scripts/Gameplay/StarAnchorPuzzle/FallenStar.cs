using UnityEngine;

public class FallenStar : MonoBehaviour, IInteractable
{
    [Header("Star Identity")]
    [SerializeField] private string starID;

    public string StarID => starID;

    public string InteractionPrompt => "[E] Collect Star";

    public void Interact()
    {
        if (StarCarryManager.Instance == null)
            return;

        if (StarCarryManager.Instance.IsCarryingStar)
        {
            Debug.Log("Already carrying a star.");
            return;
        }

        StarCarryManager.Instance.PickUpStar(starID);

        gameObject.SetActive(false);
    }
}