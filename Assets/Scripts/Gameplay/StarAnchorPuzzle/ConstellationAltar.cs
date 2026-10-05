using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class ConstellationAltar : MonoBehaviour, IInteractable
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Transform ariesCameraTarget;
    [SerializeField] private Transform defaultCameraTarget;
    [SerializeField] private float overviewOrthographicSize = 12f;

    [Header("Altar Visuals")]
    [SerializeField] private GameObject dormantVisual;
    [SerializeField] private GameObject readyVisual;
    [SerializeField] private GameObject completedVisual;
    [SerializeField] private AriesTraceController ariesTraceController;

    [Header("Timing")]
    [SerializeField] private float overviewHoldTime = 3f;

    private bool isReady = false;
    private bool isCompleted = false;
    private bool isTracing = false;

    public string InteractionPrompt
    {
        get
        {
            if (isCompleted || isTracing)
                return "";

            if (isReady)
                return "[E] Trace Constellation";

            return "";
        }
    }

    private void Start()
    {
        SetDormantVisual();
    }

    private void Update()
    {
        if (!isReady &&
            !isCompleted &&
            ConstellationManager.Instance != null &&
            ConstellationManager.Instance.IsComplete)
        {
            ActivateAltar();
        }
    }

    private void ActivateAltar()
    {
        isReady = true;

        if (dormantVisual != null)
            dormantVisual.SetActive(false);

        if (readyVisual != null)
            readyVisual.SetActive(true);

        Debug.Log("Constellation Altar is ready!");
    }

    public void Interact()
    {
        if (isCompleted || isTracing)
            return;

        if (!isReady)
        {
            Debug.Log("The altar is dormant. Restore all stars first.");
            return;
        }

        isTracing = true;

        Debug.Log("Constellation tracing begins!");

        LyraController lyra = FindAnyObjectByType<LyraController>();

        if (lyra != null)
            lyra.SetMovementLocked(true);

        StarPenController starPen =
            FindAnyObjectByType<StarPenController>();

        if (starPen != null)
            starPen.SetConstellationTracingMode(true);

        StartCoroutine(MoveAndZoomCamera());

        if (ariesTraceController != null)
            ariesTraceController.BeginTracing();
    }

    public void CompleteConstellation()
    {
        if (isCompleted)
            return;

        isCompleted = true;
        isReady = false;
        isTracing = false;

        if (dormantVisual != null)
            dormantVisual.SetActive(false);

        if (readyVisual != null)
            readyVisual.SetActive(false);

        if (completedVisual != null)
            completedVisual.SetActive(true);

        Debug.Log("Constellation completed!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.IncreaseMaxLightEnergy();
        }

        StartCoroutine(FinishConstellationSequence());
    }

    private void SetDormantVisual()
    {
        if (dormantVisual != null)
            dormantVisual.SetActive(true);

        if (readyVisual != null)
            readyVisual.SetActive(false);

        if (completedVisual != null)
            completedVisual.SetActive(false);
    }

    private IEnumerator MoveAndZoomCamera()
    {
        if (cinemachineCamera == null || ariesCameraTarget == null)
            yield break;

        float startSize =
            cinemachineCamera.Lens.OrthographicSize;

        float duration = 1.5f;
        float elapsed = 0f;

        cinemachineCamera.Follow = ariesCameraTarget;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            var lens = cinemachineCamera.Lens;

            lens.OrthographicSize = Mathf.Lerp(
                startSize,
                overviewOrthographicSize,
                t
            );

            cinemachineCamera.Lens = lens;

            yield return null;
        }
    }

    private IEnumerator FinishConstellationSequence()
    {
        // Keep the overview camera for 3 seconds
        yield return new WaitForSeconds(overviewHoldTime);

        // Return camera to normal Lyra camera target
        if (cinemachineCamera != null &&
            defaultCameraTarget != null)
        {
            cinemachineCamera.Follow = defaultCameraTarget;

            float startSize = cinemachineCamera.Lens.OrthographicSize;
            float targetSize = 5f;

            float duration = 1.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                t = Mathf.SmoothStep(0f, 1f, t);

                var lens = cinemachineCamera.Lens;

                lens.OrthographicSize = Mathf.Lerp(
                    startSize,
                    targetSize,
                    t
                );

                cinemachineCamera.Lens = lens;

                yield return null;
            }

            // Make sure it ends exactly at normal zoom
            var finalLens = cinemachineCamera.Lens;
            finalLens.OrthographicSize = targetSize;
            cinemachineCamera.Lens = finalLens;
        }

        // Restore normal Star-Pen mode
        StarPenController starPen =
            FindAnyObjectByType<StarPenController>();

        if (starPen != null)
            starPen.SetConstellationTracingMode(false);

        // Allow Lyra to move again
        LyraController lyra =
            FindAnyObjectByType<LyraController>();

        if (lyra != null)
            lyra.SetMovementLocked(false);

        // Unlock the dungeon door
        DoorTransition door =
            FindAnyObjectByType<DoorTransition>();

        if (door != null)
            door.UnlockDoor();

        Debug.Log("Aries restored. Exit door unlocked.");
    }
}