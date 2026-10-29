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
    
    [Header("Audio")]
    [SerializeField] private AudioClip tracePointSFX;
    [SerializeField] private AudioClip constellationRestoredSFX;

    private int currentPoint = 0;
    private bool isTracing = false;
    private bool completed = false;

    public bool IsCompleted => completed;
    public string ConstellationID => constellationID;

    public void BeginTracing()
    {
        if (completed)
            return;

        if (tracePoints == null || tracePoints.Length == 0)
        {
            Debug.LogError(
                "Cannot trace " + constellationID +
                " because no trace points are assigned."
            );
            return;
        }

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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(constellationRestoredSFX);
        }

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

        if (tracePoints == null || tracePoints.Length == 0)
        {
            Debug.LogError(
                "Cannot trace " + constellationID +
                " because no trace points are assigned."
            );
            return false;
        }

        // First point: player must actually start near the first star
        if (currentPoint == 0)
        {
            if (tracePoints[0] == null)
            {
                Debug.LogError(
                    "Trace point 0 is missing for " + constellationID + "."
                );
                return false;
            }

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
        {
            if (Vector2.Distance(inputPosition, end) <= pathSnapDistance)
            {
                snappedPosition = end;
                return true;
            }

            return false;
        }

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

        if (tracePoints == null || tracePoints.Length == 0)
        {
            Debug.LogError(
                "Cannot trace " + constellationID +
                " because no trace points are assigned."
            );
            return;
        }

        if (currentPoint >= tracePoints.Length)
        {
            CompleteTrace();
            return;
        }

        Transform target = tracePoints[currentPoint];
        if (target == null)
        {
            Debug.LogError(
                "Trace point " + currentPoint +
                " is missing for " + constellationID + "."
            );
            return;
        }

        if (Vector2.Distance(position, target.position) > pointReachDistance)
            return;

        Debug.Log(
            "Reached " +
            constellationID +
            " trace point " +
            (currentPoint + 1) +
            "/" +
            tracePoints.Length
        );

        currentPoint++;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(tracePointSFX);

        if (currentPoint >= tracePoints.Length)
            CompleteTrace();
    }
}