using UnityEngine;
using UnityEngine.Events;

public class CheckpointTimer : MonoBehaviour
{
    [Header("Checkpoint References")]
    public Transform pointA;
    public Transform pointB;

    [Header("Timer Settings")]
    public bool startOnAwake = false;
    public bool showDebugInfo = true;

    [Header("Events")]
    public UnityEvent<float> OnCheckpointReached;
    public UnityEvent OnTimerStart;

    private float elapsedTime = 0f;
    private bool isTimerRunning = false;
    private bool hasReachedPointB = false;
    private Transform playerTransform;

    public float ElapsedTime { get { return elapsedTime; } }
    public bool IsTimerRunning { get { return isTimerRunning; } }
    public string FormattedTime { get { return FormatTime(elapsedTime); } }

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            Debug.LogError("Player not found! Make sure your player has the 'Player' tag.");
        }

        if (startOnAwake)
        {
            StartTimer();
        }
    }

    void Update()
    {
        if (!isTimerRunning) return;

        elapsedTime += Time.deltaTime;

        // Show debug every second instead of every frame
        if (showDebugInfo && Time.frameCount % 60 == 0)
        {
            Debug.Log("Timer running: " + FormattedTime);
        }
    }

    public void StartTimer()
    {
        if (isTimerRunning) return;

        isTimerRunning = true;
        elapsedTime = 0f;
        hasReachedPointB = false;

        OnTimerStart?.Invoke();

        if (showDebugInfo)
        {
            Debug.Log("Timer started!");
        }
    }

    public void StopTimer()
    {
        if (!isTimerRunning) return;

        isTimerRunning = false;

        if (showDebugInfo)
        {
            Debug.Log("Timer stopped at: " + FormattedTime);
        }
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = false;
        hasReachedPointB = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // This won't work since CheckpointTimer doesn't have a collider
        // Use CheckpointTrigger scripts on Point A and Point B instead
        if (other.CompareTag("Player"))
        {
            CheckCheckpoint(other.transform.position);
        }
    }

    // Made public so trigger scripts can call it
    public void CheckCheckpoint(Vector3 playerPosition)
    {
        // This method is kept for backward compatibility
        // But detection is now handled by CheckpointTrigger scripts

        // Check if player reached Point A (start)
        if (pointA != null && !isTimerRunning && Vector3.Distance(playerPosition, pointA.position) < 2f)
        {
            StartTimer();
        }

        // Check if player reached Point B (end)
        if (pointB != null && isTimerRunning && !hasReachedPointB &&
            Vector3.Distance(playerPosition, pointB.position) < 2f)
        {
            hasReachedPointB = true;
            StopTimer();
            OnCheckpointReached?.Invoke(elapsedTime);

            if (showDebugInfo)
            {
                Debug.Log("Checkpoint B reached! Final time: " + FormattedTime);
            }
        }
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    void OnDrawGizmos()
    {
        // Draw Point A
        if (pointA != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pointA.position, 2f);
            Gizmos.DrawLine(pointA.position, pointA.position + Vector3.up * 3f);
        }

        // Draw Point B
        if (pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointB.position, 2f);
            Gizmos.DrawLine(pointB.position, pointB.position + Vector3.up * 3f);
        }

        // Draw line between points
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}