using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCameraTarget : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Mouse Look-Ahead")]
    public float mouseInfluence = 0.25f;
    public float maxOffset = 3f;
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (player == null)
            return;

        if (Mouse.current == null)
            return;

        if (Camera.main == null)
            return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mousePosition.z = 0f;

        Vector2 mouseDirection =
            mousePosition - player.position;

        mouseDirection = Vector2.ClampMagnitude(
            mouseDirection,
            maxOffset
        );

        Vector3 targetPosition =
            player.position +
            (Vector3)(mouseDirection * mouseInfluence);

        targetPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}