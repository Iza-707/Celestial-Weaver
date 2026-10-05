using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class StarPenController : MonoBehaviour
{
    [Header("Drawing")]
    public Material drawingMaterial;
    public float drawDistance = 5f;

    [Header("Energy")]
    private float MaxEnergy
    {
        get
        {
            if (GameManager.Instance != null)
                return GameManager.Instance.maxLightEnergy;

            return 30f;
        }
    }
    public float energyPerUnit = 20f;

    private float currentEnergy;
    public float CurrentEnergy => currentEnergy;
    public float MaxEnergyValue => MaxEnergy;

    private bool isStarPenActive = false;
    private bool constellationTracingMode = false;
    private AriesTraceController ariesTraceController;

    private Rigidbody2D rb;
    private LyraController lyraController;

    private LineRenderer currentStroke;
    private EdgeCollider2D currentCollider;

    private float strokeEnergySpent = 0f;
    private Vector3 lastDrawPosition;

    // Mobile touch tracking
    private int drawTouchId = -1;
    private int eraseTouchId = -1;

    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lyraController = GetComponent<LyraController>();

        currentEnergy = MaxEnergy;
        
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (!GameManager.Instance.starPenUnlocked)
            return;

        ToggleStarPen();

        if (!isStarPenActive)
        {
            if (currentStroke != null)
                EndStroke();

            return;
        }

        // Mobile / simulated touch
        if (TouchSimulationToggle.IsSimulationEnabled)
        {
            MobileDraw();
            MobileErase();
        }
        // Desktop
        else
        {
            Draw();
            Erase();
        }
    }

    private void ToggleStarPen()
    {
        // Desktop keyboard toggle
        if (Keyboard.current != null &&
            Keyboard.current.qKey.wasPressedThisFrame)
        {
            isStarPenActive = !isStarPenActive;

            Debug.Log(
                isStarPenActive
                    ? "Star-Pen Mode ON"
                    : "Star-Pen Mode OFF"
            );
        }

        // Mobile button toggle
        if (MobileStarPenControls.Instance != null)
        {
            // Nothing here yet.
            // Your StarPen button should call MobileToggleStarPen().
        }
    }

    public void MobileToggleStarPen()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.starPenUnlocked)
            return;

        isStarPenActive = !isStarPenActive;

        Debug.Log(
            isStarPenActive
                ? "Mobile Star-Pen ON"
                : "Mobile Star-Pen OFF"
        );
    }

    // =========================================================
    // DESKTOP
    // =========================================================

    private void Draw()
    {
        if (Mouse.current == null)
            return;

        bool isDrawing =
            Mouse.current.leftButton.isPressed;

        if (isDrawing)
        {
            if (!constellationTracingMode && currentEnergy <= 0f)return;

            if (currentStroke == null)
                StartStroke(
                    Camera.main.ScreenToWorldPoint(
                        Mouse.current.position.ReadValue()
                    )
                );

            ContinueStroke(
                Camera.main.ScreenToWorldPoint(
                    Mouse.current.position.ReadValue()
                )
            );
        }
        else if (currentStroke != null)
        {
            EndStroke();
        }
    }

    private void Erase()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.rightButton.isPressed)
            return;

        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );

        mousePosition.z = 0f;

        EraseAtPosition(mousePosition);
    }

    // =========================================================
    // MOBILE
    // =========================================================

    private void MobileDraw()
    {
        if (MobileStarPenControls.Instance == null)
            return;

        if (!MobileStarPenControls.Instance.DrawMode)
        {
            if (currentStroke != null)
                EndStroke();

            drawTouchId = -1;
            return;
        }

        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                if (drawTouchId == -1)
                {
                    drawTouchId = touch.touchId;
                }
            }

            if (touch.touchId != drawTouchId)
                continue;

            if (touch.phase == TouchPhase.Moved ||
                touch.phase == TouchPhase.Stationary)
            {
                if (currentEnergy <= 0f)
                    continue;

                Vector3 worldPosition =
                    Camera.main.ScreenToWorldPoint(
                        touch.screenPosition
                    );

                worldPosition.z = 0f;

                if (currentStroke == null)
                    StartStroke(worldPosition);

                ContinueStroke(worldPosition);
            }

            if (touch.phase == TouchPhase.Ended ||
                touch.phase == TouchPhase.Canceled)
            {
                EndStroke();
                drawTouchId = -1;
            }
        }
    }

    private void MobileErase()
    {
        if (MobileStarPenControls.Instance == null)
            return;

        if (!MobileStarPenControls.Instance.EraseMode)
        {
            eraseTouchId = -1;
            return;
        }

        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                if (eraseTouchId == -1)
                {
                    eraseTouchId = touch.touchId;
                }
            }

            if (touch.touchId != eraseTouchId)
                continue;

            if (touch.phase == TouchPhase.Moved ||
                touch.phase == TouchPhase.Stationary)
            {
                Vector3 worldPosition =
                    Camera.main.ScreenToWorldPoint(
                        touch.screenPosition
                    );

                worldPosition.z = 0f;

                EraseAtPosition(worldPosition);
            }

            if (touch.phase == TouchPhase.Ended ||
                touch.phase == TouchPhase.Canceled)
            {
                eraseTouchId = -1;
            }
        }
    }

    // =========================================================
    // SHARED DRAWING
    // =========================================================


    public void SetConstellationTracingMode(bool enabled)
    {
        constellationTracingMode = enabled;

        if (enabled)
        {
            ariesTraceController =
                FindAnyObjectByType<AriesTraceController>();
        }
        else
        {
            ariesTraceController = null;
        }

        Debug.Log(
            enabled
                ? "Star-Pen: Constellation Tracing Mode"
                : "Star-Pen: Normal Mode"
        );
    }
    private void StartStroke(Vector3 screenWorldPosition)
    {
        GameObject strokeObject =
            new GameObject("Light Construct");

        int groundLayer =
            LayerMask.NameToLayer("Ground");

        if (groundLayer != -1)
            strokeObject.layer = groundLayer;

        LightConstruct construct =
            strokeObject.AddComponent<LightConstruct>();

        currentStroke =
            strokeObject.AddComponent<LineRenderer>();

        if (!constellationTracingMode)
        {
            EdgeCollider2D edgeCollider =
                strokeObject.AddComponent<EdgeCollider2D>();

            edgeCollider.edgeRadius = 0.15f;
            edgeCollider.isTrigger = false;

            currentCollider = edgeCollider;
        }
        else
        {
            currentCollider = null;
        }

        currentStroke.positionCount = 0;
        if (constellationTracingMode)
        {
            currentStroke.startWidth = 0.25f;
            currentStroke.endWidth = 0.25f;
        }
        else
        {
            currentStroke.startWidth = 0.15f;
            currentStroke.endWidth = 0.15f;
        }

        currentStroke.material = drawingMaterial;
        currentStroke.useWorldSpace = true;

        currentStroke.sortingLayerName = "Effects";
        currentStroke.sortingOrder = 100;

        strokeEnergySpent = 0f;

        if (constellationTracingMode)
        {
            screenWorldPosition.z = 0f;

            if (ariesTraceController != null)
            {
                if (!ariesTraceController.TryGetSnappedPosition(
                    screenWorldPosition,
                    out Vector3 snappedPosition))
                {
                    return;
                }

                screenWorldPosition = snappedPosition;
            }

            lastDrawPosition = screenWorldPosition;
        }
        else
        {
            Vector2 direction =
                screenWorldPosition - transform.position;

            if (direction.magnitude > drawDistance)
            {
                direction =
                    direction.normalized * drawDistance;
            }

            lastDrawPosition =
                transform.position + (Vector3)direction;
        }

        currentStroke.positionCount = 1;

        currentStroke.SetPosition(
            0,
            lastDrawPosition
        );
    }

    private void ContinueStroke(Vector3 screenWorldPosition)
    {
        if (currentStroke == null)
            return;

        Vector3 drawPosition;

        if (constellationTracingMode)
        {
            screenWorldPosition.z = 0f;

            if (ariesTraceController != null)
            {
                if (!ariesTraceController.TryGetSnappedPosition(
                    screenWorldPosition,
                    out Vector3 snappedPosition))
                {
                    return;
                }

                screenWorldPosition = snappedPosition;
            }

            drawPosition = screenWorldPosition;

            if (ariesTraceController != null)
            {
                ariesTraceController.CheckTracePosition(drawPosition);
            }
        }
        else
        {
            Vector2 direction =
                screenWorldPosition - transform.position;

            if (direction.magnitude > drawDistance)
            {
                direction =
                    direction.normalized * drawDistance;
            }

            drawPosition =
                transform.position + (Vector3)direction;
        }

        float distanceMoved =
            Vector3.Distance(
                lastDrawPosition,
                drawPosition
            );

        if (distanceMoved < 0.05f)
            return;

        float energyCost = distanceMoved * energyPerUnit;

        if (!constellationTracingMode)
        {
            if (currentEnergy <= 0f)
                return;

            if (energyCost > currentEnergy)
                energyCost = currentEnergy;

            currentEnergy -= energyCost;
            strokeEnergySpent += energyCost;
        }

        currentStroke.positionCount++;

        currentStroke.SetPosition(
            currentStroke.positionCount - 1,
            drawPosition
        );

        UpdateCollider();

        lastDrawPosition = drawPosition;
    }

    private void UpdateCollider()
    {
        if (currentStroke == null ||
            currentCollider == null)
            return;

        Vector3[] positions =
            new Vector3[currentStroke.positionCount];

        currentStroke.GetPositions(positions);

        Vector2[] colliderPoints =
            new Vector2[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            colliderPoints[i] = positions[i];
        }

        currentCollider.points = colliderPoints;
    }

    private void EndStroke()
    {
        if (currentStroke != null)
        {
            LightConstruct construct =
                currentStroke.GetComponent<LightConstruct>();

            if (construct != null)
                construct.energySpent = strokeEnergySpent;
        }

        currentStroke = null;
        currentCollider = null;
        strokeEnergySpent = 0f;
    }

    private void EraseAtPosition(Vector3 worldPosition)
    {
        float eraseRadius = 0.25f;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                worldPosition,
                eraseRadius
            );

        foreach (Collider2D hit in hits)
        {
            if (hit.transform.name != "Light Construct")
                continue;

            LightConstruct construct =
                hit.GetComponent<LightConstruct>();

            if (construct == null)
                continue;

            currentEnergy += construct.energySpent;

            currentEnergy =
                Mathf.Min(
                    currentEnergy,
                    MaxEnergy
                );

            Destroy(hit.gameObject);
        }
    }
}