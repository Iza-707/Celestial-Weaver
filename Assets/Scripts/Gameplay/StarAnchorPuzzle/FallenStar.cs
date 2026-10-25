using UnityEngine;

public class FallenStar : MonoBehaviour, IInteractable
{
    [Header("Star Identity")]
    [SerializeField] private string starID;

    [Header("Sound")]
    [SerializeField] private AudioClip pickupSFX;
    [Range(0f, 1f)]
    [SerializeField] private float pickupSFXVolume = 1f;

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

        if (AudioManager.Instance != null && pickupSFX != null)
            AudioManager.Instance.PlaySFX(pickupSFX, pickupSFXVolume);

        gameObject.SetActive(false);
    }
}