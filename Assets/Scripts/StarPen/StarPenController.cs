using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

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
    private ConstellationTraceController traceController;
    private Rigidbody2D rb;
    private LyraController lyraController;

    private LineRenderer currentStroke;
    private EdgeCollider2D currentCollider;

    private float strokeEnergySpent = 0f;
    private Vector3 lastDrawPosition;

    // Mobile touch tracking
    private int drawTouchId = -1;
    private int eraseTouchId = -1;
    private bool ignoreInputUntilReleased;
    private readonly List<RaycastResult> uiRaycastResults = new List<RaycastResult>();
    private PointerEventData touchPointerEventData;
    private const float EraseRadiusPixels = 24f;

    // check if the application is running on a mobile platform
    private bool IsMobilePlatform =>
    Application.isMobilePlatform;

    

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

        if (ignoreInputUntilReleased)
        {
            bool mousePressed = Mouse.current != null &&
                (Mouse.current.leftButton.isPressed ||
                 Mouse.current.rightButton.isPressed);

            if (mousePressed || Touch.activeTouches.Count > 0)
                return;

            ignoreInputUntilReleased = false;
        }

        ToggleStarPen();

        bool touchModeActive = MobileStarPenControls.Instance != null &&
            (MobileStarPenControls.Instance.DrawMode ||
             MobileStarPenControls.Instance.EraseMode);

        if (!isStarPenActive && !touchModeActive)
        {
            if (currentStroke != null)
                EndStroke();

            return;
        }

        if (IsMobilePlatform || Touch.activeTouches.Count > 0)
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
        // Prevent turning off the StarPen during constellation tracing.
        if (constellationTracingMode && isStarPenActive)
            return;

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
    }



    public void MobileToggleStarPen()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.starPenUnlocked)
            return;

        // Prevent turning off the StarPen during constellation tracing.
        if (constellationTracingMode && isStarPenActive)
            return;

        isStarPenActive = !isStarPenActive;

        Debug.Log(
            isStarPenActive
                ? "Mobile Star-Pen ON"
                : "Mobile Star-Pen OFF"
        );
    }

    public void ActivateFromPickup()
    {
        isStarPenActive = true;

        if (MobileStarPenControls.Instance != null)
            MobileStarPenControls.Instance.SelectDrawMode();
    }


    // =========================================================
    // DESKTOP
    // =========================================================

    private void Draw()
    {
        if (Mouse.current == null)
            return;

        MobileStarPenControls controls = MobileStarPenControls.Instance;
        if (controls != null ? !controls.DrawMode : !isStarPenActive)
        {
            if (currentStroke != null)
                EndStroke();

            return;
        }

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

        bool eraseMode = MobileStarPenControls.Instance != null &&
            MobileStarPenControls.Instance.EraseMode;
        bool erasePressed = eraseMode
            ? Mouse.current.leftButton.isPressed
            : Mouse.current.rightButton.isPressed;

        if (!erasePressed)
            return;

        EraseAtScreenPosition(Mouse.current.position.ReadValue());
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
                if (drawTouchId != -1 ||
                    IsTouchOverInteractiveUI(touch.screenPosition) ||
                    IsTouchOverJoystick(touch.screenPosition))
                    continue;

                drawTouchId = touch.touchId;

                if (!constellationTracingMode && currentEnergy <= 0f)
                    continue;

                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touch.screenPosition);
                worldPosition.z = 0f;
                StartStroke(worldPosition);
                continue;
            }

            if (touch.touchId != drawTouchId)
                continue;

            if (touch.phase == TouchPhase.Moved ||
                touch.phase == TouchPhase.Stationary)
            {
                if (IsTouchOverJoystick(touch.screenPosition))
                {
                    EndStroke();
                    continue;
                }

                if (!constellationTracingMode && currentEnergy <= 0f)
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

        if (drawTouchId != -1 && !IsTouchActive(drawTouchId))
        {
            EndStroke();
            drawTouchId = -1;
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
                if (eraseTouchId != -1 || IsTouchOverInteractiveUI(touch.screenPosition))
                    continue;

                eraseTouchId = touch.touchId;

                EraseAtScreenPosition(touch.screenPosition);
                continue;
            }

            if (touch.touchId != eraseTouchId)
                continue;

            if (touch.phase == TouchPhase.Moved ||
                touch.phase == TouchPhase.Stationary)
            {
                EraseAtScreenPosition(touch.screenPosition);
            }

            if (touch.phase == TouchPhase.Ended ||
                touch.phase == TouchPhase.Canceled)
            {
                eraseTouchId = -1;
            }
        }

        if (eraseTouchId != -1 && !IsTouchActive(eraseTouchId))
            eraseTouchId = -1;
    }

    private bool IsTouchOverInteractiveUI(Vector2 screenPosition)
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
            return false;

        if (touchPointerEventData == null)
            touchPointerEventData = new PointerEventData(eventSystem);

        touchPointerEventData.position = screenPosition;
        uiRaycastResults.Clear();
        eventSystem.RaycastAll(touchPointerEventData, uiRaycastResults);

        foreach (RaycastResult result in uiRaycastResults)
        {
            if (result.gameObject.GetComponentInParent<Selectable>() != null)
                return true;
        }

        return false;
    }

    private bool IsTouchOverJoystick(Vector2 screenPosition)
    {
        return MobileInputProvider.Instance != null &&
            MobileInputProvider.Instance.IsTouchOverJoystick(screenPosition);
    }

    private bool IsTouchActive(int touchId)
    {
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.touchId == touchId)
                return true;
        }

        return false;
    }

    // =========================================================
    // SHARED DRAWING
    // =========================================================


    public void SetConstellationTracingMode(bool enabled)
    {
        constellationTracingMode = enabled;

        if (enabled)
        {
            traceController =
                FindAnyObjectByType<ConstellationTraceController>();

            isStarPenActive = true;

            if (MobileStarPenControls.Instance != null)
                MobileStarPenControls.Instance.SelectDrawMode();

            if (traceController == null)
            {
                Debug.LogError(
                    "Cannot start constellation tracing without a " +
                    "ConstellationTraceController."
                );
            }
        }
        else
        {
            traceController = null;
        }

        Debug.Log(
            enabled
                ? "Star-Pen: Constellation Tracing Mode"
                : "Star-Pen: Normal Mode"
        );
    }

    public void IgnoreInputUntilReleased()
    {
        ignoreInputUntilReleased = true;
        drawTouchId = -1;
        eraseTouchId = -1;

        if (currentStroke != null)
            EndStroke();
    }

    private void StartStroke(Vector3 screenWorldPosition)
    {
        if (constellationTracingMode)
        {
            screenWorldPosition.z = 0f;

            if (traceController == null ||
                !traceController.TryGetSnappedPosition(
                    screenWorldPosition,
                    out Vector3 snappedPosition))
            {
                return;
            }

            screenWorldPosition = snappedPosition;
        }

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

            if (traceController != null)
            {
                if (!traceController.TryGetSnappedPosition(
                    screenWorldPosition,
                    out Vector3 snappedPosition))
                {
                    return;
                }

                screenWorldPosition = snappedPosition;
            }

            drawPosition = screenWorldPosition;

            if (traceController != null)
                traceController.CheckTracePosition(drawPosition);
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

    private void EraseAtScreenPosition(Vector2 screenPosition)
    {
        Camera camera = Camera.main;
        if (camera == null)
            return;

        LightConstruct[] constructs =
            FindObjectsByType<LightConstruct>(FindObjectsSortMode.None);

        foreach (LightConstruct construct in constructs)
        {
            LineRenderer line = construct.GetComponent<LineRenderer>();
            if (line == null || line.positionCount == 0)
                continue;

            Vector3 firstPosition = camera.WorldToScreenPoint(line.GetPosition(0));
            bool hitStroke = Vector2.Distance(screenPosition, firstPosition) <= EraseRadiusPixels;

            for (int i = 1; i < line.positionCount && !hitStroke; i++)
            {
                Vector3 nextPosition = camera.WorldToScreenPoint(line.GetPosition(i));
                hitStroke = DistanceToSegment(
                    screenPosition,
                    firstPosition,
                    nextPosition
                ) <= EraseRadiusPixels;
                firstPosition = nextPosition;
            }

            if (!hitStroke)
                continue;

            currentEnergy += construct.energySpent;

            currentEnergy =
                Mathf.Min(
                    currentEnergy,
                    MaxEnergy
                );

            Destroy(construct.gameObject);
        }
    }

    private float DistanceToSegment(Vector2 point, Vector2 start, Vector2 end)
    {
        Vector2 segment = end - start;
        float segmentLengthSquared = segment.sqrMagnitude;
        if (segmentLengthSquared <= Mathf.Epsilon)
            return Vector2.Distance(point, start);

        float t = Mathf.Clamp01(Vector2.Dot(point - start, segment) / segmentLengthSquared);
        Vector2 closestPoint = start + segment * t;
        return Vector2.Distance(point, closestPoint);
    }
}