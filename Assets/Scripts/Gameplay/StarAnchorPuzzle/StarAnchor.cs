
using UnityEngine;

public class StarAnchor : MonoBehaviour, IInteractable
{
    [Header("Star Identity")]
    [SerializeField] private string requiredStarID;

    [Header("Visual")]
    [SerializeField] private GameObject filledVisual;
    [SerializeField] private ParticleSystem anchorGlow;

    [Header("Star Tracking")]
    [SerializeField] private ParticleSystem trackingParticles;

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

    private void Update()
    {
        bool shouldTrack =
            !isFilled &&
            StarCarryManager.Instance != null &&
            StarCarryManager.Instance.IsCarryingStar &&
            StarCarryManager.Instance.CarriedStarID == requiredStarID;

        if (trackingParticles == null)
            return;

        if (shouldTrack && !trackingParticles.isPlaying)
        {
            trackingParticles.Play();
        }
        else if (!shouldTrack && trackingParticles.isPlaying)
        {
            trackingParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
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

        if (trackingParticles != null)
        {
            trackingParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }

        if (anchorGlow != null)
        {
            anchorGlow.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }

        StarCarryManager.Instance.ClearStar();

        if (ConstellationManager.Instance != null)
        {
            ConstellationManager.Instance.StarPlaced();
        }

        if (filledVisual != null)
            filledVisual.SetActive(true);
    }
}
