using UnityEngine;
using Unity.Cinemachine;

public class ConstellationAltar : MonoBehaviour, IInteractable
{
[Header("Camera")]
[SerializeField] private CinemachineCamera cinemachineCamera;
[SerializeField] private Transform ariesCameraTarget;
[SerializeField] private float overviewOrthographicSize = 12f;
[SerializeField] private float cameraMoveSpeed = 3f;
[SerializeField] private float cameraZoomSpeed = 2f;

    [Header("Altar Visuals")]
    [SerializeField] private GameObject dormantVisual;
    [SerializeField] private GameObject readyVisual;
    [SerializeField] private GameObject completedVisual;
    [SerializeField] private AriesTraceController ariesTraceController;

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
        // Wait until all 4 stars have been restored
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
        {
            lyra.SetMovementLocked(true);
        }

        StarPenController starPen = FindAnyObjectByType<StarPenController>();

        if (starPen != null)
        {
            starPen.SetConstellationTracingMode(true);
        }

        StartCoroutine(MoveAndZoomCamera());

        if (ariesTraceController != null)
        {
            ariesTraceController.BeginTracing();
        }
    }

    public void CompleteConstellation()
    {
        isCompleted = true;
        isReady = false;

        if (dormantVisual != null)
            dormantVisual.SetActive(false);

        if (readyVisual != null)
            readyVisual.SetActive(false);

        if (completedVisual != null)
            completedVisual.SetActive(true);

        Debug.Log("Constellation completed!");
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
    private System.Collections.IEnumerator MoveAndZoomCamera()
    {
        if (cinemachineCamera == null || ariesCameraTarget == null)
            yield break;

        float startSize = cinemachineCamera.Lens.OrthographicSize;
        float duration = 1.5f;
        float elapsed = 0f;

        // Tell Cinemachine to follow the Aries overview target
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
}