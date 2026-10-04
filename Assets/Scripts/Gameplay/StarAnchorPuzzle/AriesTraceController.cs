using UnityEngine;

public class AriesTraceController : MonoBehaviour
{
    [Header("Trace Points")]
    [SerializeField] private Transform[] tracePoints;

    [Header("Tracing")]
    [SerializeField] private float pointReachDistance = 1.2f;

    private int currentPoint = 0;
    private bool isTracing = false;
    private bool completed = false;

    private LyraController lyra;

    public bool IsCompleted => completed;

    public void BeginTracing()
    {
        if (completed)
            return;

        lyra = FindAnyObjectByType<LyraController>();

        currentPoint = 0;
        isTracing = true;

        Debug.Log("Aries tracing started.");
    }

    private void Update()
    {
        if (!isTracing || completed)
            return;

        if (lyra == null)
            return;

        if (currentPoint >= tracePoints.Length)
        {
            CompleteTrace();
            return;
        }

        float distance = Vector2.Distance(
            lyra.transform.position,
            tracePoints[currentPoint].position
        );

        if (distance <= pointReachDistance)
        {
            Debug.Log(
                "Reached Aries trace point " +
                (currentPoint + 1) +
                "/" +
                tracePoints.Length
            );

            currentPoint++;
        }
    }

    private void CompleteTrace()
    {
        isTracing = false;
        completed = true;

        Debug.Log("ARIES CONSTELLATION RESTORED!");

        ConstellationAltar altar =
            FindAnyObjectByType<ConstellationAltar>();

        if (altar != null)
        {
            altar.CompleteConstellation();
        }
    }
}