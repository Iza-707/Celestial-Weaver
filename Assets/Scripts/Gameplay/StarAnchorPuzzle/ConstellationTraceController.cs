using UnityEngine;

public class ConstellationTraceController : MonoBehaviour
{
    [Header("Constellation")]
    [SerializeField] private string constellationID = "Aries";

    [Header("Trace Points")]
    [SerializeField] private Transform[] tracePoints;

    [Header("Tracing")]
    [SerializeField] private float pointReachDistance = 1.2f;
    [SerializeField] private float pathSnapDistance = 1.5f;

    private int currentPoint = 0;
    private bool isTracing = false;
    private bool completed = false;

    public bool IsCompleted => completed;
    public string ConstellationID => constellationID;

    public void BeginTracing()
    {
        if (completed)
            return;

        currentPoint = 0;
        isTracing = true;

        Debug.Log(constellationID + " tracing started.");
    }

    private void Update()
    {
        if (!isTracing || completed)
            return;

        if (currentPoint >= tracePoints.Length)
        {
            CompleteTrace();
        }
    }

    private void CompleteTrace()
    {
        isTracing = false;
        completed = true;

        Debug.Log(
            constellationID.ToUpper() +
            " CONSTELLATION RESTORED!"
        );

        ConstellationAltar altar =
            FindAnyObjectByType<ConstellationAltar>();

        if (altar != null)
        {
            altar.CompleteConstellation();
        }
    }

    public bool TryGetSnappedPosition(
        Vector3 inputPosition,
        out Vector3 snappedPosition)
    {
        snappedPosition = inputPosition;

        if (!isTracing || completed)
            return false;

        // First point: player must actually start near the first star
        if (currentPoint == 0)
        {
            float distanceToStart = Vector2.Distance(
                inputPosition,
                tracePoints[0].position
            );

            if (distanceToStart <= pathSnapDistance)
            {
                snappedPosition = tracePoints[0].position;
                snappedPosition.z = 0f;
                return true;
            }

            return false;
        }

        if (currentPoint >= tracePoints.Length)
            return false;

        Vector3 start =
            tracePoints[currentPoint - 1].position;

        Vector3 end =
            tracePoints[currentPoint].position;

        start.z = 0f;
        end.z = 0f;

        Vector3 segment = end - start;

        float segmentLengthSquared =
            segment.sqrMagnitude;

        if (segmentLengthSquared <= 0.001f)
            return false;

        float t = Vector3.Dot(
            inputPosition - start,
            segment
        ) / segmentLengthSquared;

        t = Mathf.Clamp01(t);

        Vector3 closestPoint =
            start + segment * t;

        closestPoint.z = 0f;

        float distance = Vector2.Distance(
            inputPosition,
            closestPoint
        );

        if (distance <= pathSnapDistance)
        {
            snappedPosition = closestPoint;
            return true;
        }

        return false;
    }

    public void CheckTracePosition(Vector3 position)
    {
        if (!isTracing || completed)
            return;

        if (currentPoint >= tracePoints.Length)
        {
            CompleteTrace();
            return;
        }

        float distance = Vector2.Distance(
            position,
            tracePoints[currentPoint].position
        );

        if (distance <= pointReachDistance)
        {
            Debug.Log(
                "Reached " +
                constellationID +
                " trace point " +
                (currentPoint + 1) +
                "/" +
                tracePoints.Length
            );

            currentPoint++;

            if (currentPoint >= tracePoints.Length)
            {
                CompleteTrace();
            }
        }
    }
}