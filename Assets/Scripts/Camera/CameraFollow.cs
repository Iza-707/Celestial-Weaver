using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Follow Settings")]
    public float followSpeed = 5f;

    [Header("Mouse Look-Ahead")]
    public float mouseInfluence = 0.3f;
    public float maxMouseOffset = 3f;

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = transform.position;

        // Follow Lyra horizontally
        targetPosition.x = target.position.x;

        if (Mouse.current != null && Camera.main != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );

            mousePosition.z = 0f;

            Vector2 mouseDirection =
                mousePosition - target.position;

            mouseDirection = Vector2.ClampMagnitude(
                mouseDirection,
                maxMouseOffset
            );

            targetPosition.x +=
                mouseDirection.x * mouseInfluence;

            targetPosition.y +=
                mouseDirection.y * mouseInfluence;
        }

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}