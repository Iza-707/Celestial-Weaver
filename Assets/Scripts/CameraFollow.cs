using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Follow Settings")]
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPosition = transform.position;

        targetPosition.x = target.position.x;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}