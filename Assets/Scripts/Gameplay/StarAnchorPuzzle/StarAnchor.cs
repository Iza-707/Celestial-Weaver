using UnityEngine;

public class StarAnchor : MonoBehaviour, IInteractable
{
    [Header("Star Identity")]
    [SerializeField] private string requiredStarID;

    [Header("Visual")]
    [SerializeField] private GameObject filledVisual;

    private bool isFilled = false;

    public string InteractionPrompt
    {
        get
        {
            if (isFilled)
                return "";

            if (StarCarryManager.Instance != null &&
                StarCarryManager.Instance.IsCarryingStar)
            {
                return "[E] Place Star";
            }

            return "[E] Requires " + requiredStarID;
        }
    }

    public void Interact()
    {
        if (isFilled)
            return;

        if (StarCarryManager.Instance == null)
            return;

        if (!StarCarryManager.Instance.IsCarryingStar)
        {
            Debug.Log("No star currently carried.");
            return;
        }

        string carriedStar =
            StarCarryManager.Instance.CarriedStarID;

        // WRONG STAR
        if (carriedStar != requiredStarID)
        {
            Debug.Log(
                "Wrong star! This anchor requires " +
                requiredStarID +
                ", but Lyra is carrying " +
                carriedStar
            );

            return;
        }

        // CORRECT STAR
        isFilled = true;

        StarCarryManager.Instance.ClearStar();

        if (ConstellationManager.Instance != null)
        {
            ConstellationManager.Instance.StarPlaced();
        }

        if (filledVisual != null)
            filledVisual.SetActive(true);
    }
}