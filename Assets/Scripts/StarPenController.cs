using UnityEngine;
using UnityEngine.InputSystem;

public class StarPenController : MonoBehaviour
{
    [Header("Drawing")]
    public GameObject lightConstructPrefab;
    public Transform drawingPoint;
    public float drawDistance = 5f;

    [Header("Energy")]
    public float maxEnergy = 100f;
    public float energyPerSecond = 10f;

    private float currentEnergy;
    private bool isStarPenActive = false;

    private Rigidbody2D rb;
    private LyraController lyraController;

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
        }
    }

    void ToggleStarPen()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            // Lyra must be standing still
            if (Mathf.Abs(rb.linearVelocity.x) > 0.01f)
                return;

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

        if (!Mouse.current.leftButton.isPressed)
            return;

        if (currentEnergy <= 0f)
            return;

        currentEnergy -= energyPerSecond * Time.deltaTime;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mousePosition.z = 0f;

        Vector2 direction = mousePosition - transform.position;

        if (direction.magnitude > drawDistance)
        {
            direction = direction.normalized * drawDistance;
        }

        Vector3 spawnPosition = transform.position + (Vector3)direction;

        if (lightConstructPrefab != null)
        {
            Instantiate(
                lightConstructPrefab,
                spawnPosition,
                Quaternion.identity
            );
        }
    }
}