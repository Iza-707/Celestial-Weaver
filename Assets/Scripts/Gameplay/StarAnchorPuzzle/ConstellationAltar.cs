using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class ConstellationAltar : MonoBehaviour, IInteractable
{
    [Header("Constellation")]
    [SerializeField] private string constellationID = "Aries";
    [SerializeField] private ConstellationTraceController traceController;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform defaultCameraTarget;
    [SerializeField] private float overviewOrthographicSize = 12f;

    [Header("Altar Visuals")]
    [SerializeField] private GameObject dormantVisual;
    [SerializeField] private GameObject readyVisual;
    [SerializeField] private GameObject completedVisual;

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

        Debug.Log(
            constellationID +
            " Constellation Altar is ready!"
        );
    }

    public void Interact()
    {
        if (isCompleted || isTracing)
            return;

        if (!isReady)
        {
            Debug.Log(
                "The altar is dormant. Restore all stars first."
            );
            return;
        }

        if (traceController == null)
        {
            Debug.LogError(
                "No ConstellationTraceController assigned to " +
                gameObject.name
            );
            return;
        }

        isTracing = true;

        Debug.Log(
            constellationID +
            " constellation tracing begins!"
        );

        LyraController lyra =
            FindAnyObjectByType<LyraController>();

        if (lyra != null)
            lyra.SetMovementLocked(true);

        StarPenController starPen =
            FindAnyObjectByType<StarPenController>();

        if (starPen != null)
            starPen.SetConstellationTracingMode(true);

        StartCoroutine(MoveAndZoomCamera());

        traceController.BeginTracing();
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

        Debug.Log(
            constellationID +
            " constellation completed!"
        );

        if (GameManager.Instance != null)
        {
            GameManager.Instance.IncreaseMaxLightEnergy();
            GameManager.Instance.CompleteConstellation(constellationID);

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
        if (cinemachineCamera == null ||
            cameraTarget == null)
            yield break;

        float startSize =
            cinemachineCamera.Lens.OrthographicSize;

        float duration = 1.5f;
        float elapsed = 0f;

        cinemachineCamera.Follow = cameraTarget;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(elapsed / duration);

            t = Mathf.SmoothStep(0f, 1f, t);

            var lens = cinemachineCamera.Lens;

            lens.OrthographicSize =
                Mathf.Lerp(
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
        // Keep the overview camera for a few seconds
        yield return new WaitForSeconds(
            overviewHoldTime
        );

        // Return camera to normal Lyra target
        if (cinemachineCamera != null &&
            defaultCameraTarget != null)
        {
            cinemachineCamera.Follow =
                defaultCameraTarget;

            float startSize =
                cinemachineCamera.Lens.OrthographicSize;

            float targetSize = 5f;

            float duration = 1.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t =
                    Mathf.Clamp01(elapsed / duration);

                t = Mathf.SmoothStep(0f, 1f, t);

                var lens = cinemachineCamera.Lens;

                lens.OrthographicSize =
                    Mathf.Lerp(
                        startSize,
                        targetSize,
                        t
                    );

                cinemachineCamera.Lens = lens;

                yield return null;
            }

            var finalLens =
                cinemachineCamera.Lens;

            finalLens.OrthographicSize =
                targetSize;

            cinemachineCamera.Lens =
                finalLens;
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

        Debug.Log(
            constellationID +
            " restored. Exit door unlocked."
        );
    }
}