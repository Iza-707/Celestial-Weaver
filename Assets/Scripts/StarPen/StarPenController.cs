using UnityEngine;
using UnityEngine.InputSystem;

public class StarPenController : MonoBehaviour
{
    [Header("Drawing")]
    public Material drawingMaterial;
    public float drawDistance = 5f;

    [Header("Energy")]
    public float maxEnergy = 30f;
    public float energyPerUnit = 20f;

    private float currentEnergy;
    public float CurrentEnergy => currentEnergy;

    private bool isStarPenActive = false;

    private Rigidbody2D rb;
    private LyraController lyraController;

    private LineRenderer currentStroke;
    private EdgeCollider2D currentCollider;

    private float strokeEnergySpent = 0f;
    private Vector3 lastDrawPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lyraController = GetComponent<LyraController>();

        currentEnergy = maxEnergy;
    }

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (!GameManager.Instance.starPenUnlocked)
            return;

        ToggleStarPen();

        if (isStarPenActive)
        {
            Draw();
            Erase();
        }
    }

    void ToggleStarPen()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {

            isStarPenActive = !isStarPenActive;

            Debug.Log(
                isStarPenActive
                ? "Star-Pen Mode ON"
                : "Star-Pen Mode OFF"
            );
        }
    }

    void Draw()
    {
        if (Mouse.current == null)
        return;

        bool isDrawing = Mouse.current.leftButton.isPressed;

        if (isDrawing)
        {
            if (currentEnergy <= 0f)
                return;

            if (currentStroke == null)
            {
                StartStroke();
            }

            ContinueStroke();
        }
        else if (currentStroke != null)
        {
            EndStroke();
        }
    }

    void StartStroke()
    {
        GameObject strokeObject = new GameObject("Light Construct");
        
        int groundLayer = LayerMask.NameToLayer("Ground");
        
        if (groundLayer != -1)
        {
            strokeObject.layer = groundLayer;
        }

        LightConstruct construct = strokeObject.AddComponent<LightConstruct>();

        currentStroke = strokeObject.AddComponent<LineRenderer>();
        
        EdgeCollider2D edgeCollider = strokeObject.AddComponent<EdgeCollider2D>();

        edgeCollider.edgeRadius = 0.15f;
        edgeCollider.isTrigger = false;
        currentCollider = edgeCollider;
        currentStroke.positionCount = 0;
        currentStroke.startWidth = 0.15f;
        currentStroke.endWidth = 0.15f;

        currentStroke.material = drawingMaterial;

        currentStroke.useWorldSpace = true;
        
        currentStroke.sortingLayerName = "Effects";
        currentStroke.sortingOrder = 10;

        strokeEnergySpent = 0f;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mousePosition.z = 0f;

        Vector2 direction = mousePosition - transform.position;

        if (direction.magnitude > drawDistance)
        {
            direction = direction.normalized * drawDistance;
        }

        lastDrawPosition = transform.position + (Vector3)direction;

        currentStroke.positionCount = 1;
        currentStroke.SetPosition(0, lastDrawPosition);
    }

    void ContinueStroke()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mousePosition.z = 0f;

        Vector2 direction = mousePosition - transform.position;

        if (direction.magnitude > drawDistance)
        {
            direction = direction.normalized * drawDistance;
        }

        Vector3 drawPosition = transform.position + (Vector3)direction;

        float distanceMoved = Vector3.Distance(
            lastDrawPosition,
            drawPosition
        );

        if (distanceMoved < 0.05f)
            return;

        float energyCost = distanceMoved * energyPerUnit;

        if (currentEnergy <= 0f)
            return;

        if (energyCost > currentEnergy)
        {
            energyCost = currentEnergy;
        }

        currentEnergy -= energyCost;
        strokeEnergySpent += energyCost;

        currentStroke.positionCount++;

        currentStroke.SetPosition(
            currentStroke.positionCount - 1,
            drawPosition
        );

        UpdateCollider();

        lastDrawPosition = drawPosition;
    }

    void UpdateCollider()
    {
        if (currentStroke == null || currentCollider == null)
            return;

        Vector3[] positions = new Vector3[currentStroke.positionCount];

        currentStroke.GetPositions(positions);

        Vector2[] colliderPoints = new Vector2[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            colliderPoints[i] = positions[i];
        }

        currentCollider.points = colliderPoints;
    }

    void EndStroke()
    {
        if (currentStroke != null)
        {
            LightConstruct construct =
                currentStroke.GetComponent<LightConstruct>();

            if (construct != null)
            {
                construct.energySpent = strokeEnergySpent;
            }
        }

        currentStroke = null;
        currentCollider = null;
        strokeEnergySpent = 0f;
    }

    void Erase()
    {
        if (Mouse.current == null)
            return;

        if (!Mouse.current.rightButton.isPressed)
            return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mousePosition.z = 0f;

        float eraseRadius = 0.25f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            mousePosition,
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

            currentEnergy = Mathf.Min(
                currentEnergy,
                maxEnergy
            );

            Destroy(hit.gameObject);
        }
    }
}