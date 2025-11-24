using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public enum CheckpointType { PointA, PointB }

    [Header("Configuration")]
    public CheckpointType checkpointType;

    [Header("Visual Feedback")]
    public bool showDebug = true;

    private CheckpointTimer checkpointTimer;

    void Start()
    {
        checkpointTimer = FindObjectOfType<CheckpointTimer>();

        if (checkpointTimer == null)
        {
            Debug.LogError("CheckpointTimer not found in scene! Make sure you have a CheckpointTimer GameObject.");
        }

        if (showDebug)
        {
            Debug.Log($"{gameObject.name} initialized as {checkpointType}");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Only respond to player
        if (!other.CompareTag("Player"))
            return;

        if (checkpointTimer == null)
        {
            Debug.LogError("CheckpointTimer reference is null!");
            return;
        }

        if (checkpointType == CheckpointType.PointA)
        {
            // Start timer
            if (!checkpointTimer.IsTimerRunning)
            {
                if (showDebug)
                {
                    Debug.Log("✅ Player reached Point A - Starting timer!");
                }

                checkpointTimer.StartTimer();

                // Play start sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayTimerStartSound();
                }
                else
                {
                    Debug.LogWarning("AudioManager not found!");
                }
            }
        }
        else if (checkpointType == CheckpointType.PointB)
        {
            // Stop timer and complete level
            if (checkpointTimer.IsTimerRunning)
            {
                if (showDebug)
                {
                    Debug.Log("✅ Player reached Point B - Level complete!");
                }

                checkpointTimer.StopTimer();

                // Play end sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayTimerEndSound();
                }
                else
                {
                    Debug.LogWarning("AudioManager not found!");
                }

                // Trigger completion event
                checkpointTimer.OnCheckpointReached?.Invoke(checkpointTimer.ElapsedTime);
            }
        }
    }

    void OnDrawGizmos()
    {
        // Visual feedback in editor
        Gizmos.color = (checkpointType == CheckpointType.PointA) ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f);

        // Draw upward line
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 3f);
    }

    void OnDrawGizmosSelected()
    {
        // Show trigger area when selected
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Gizmos.color = (checkpointType == CheckpointType.PointA) ?
                new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
        }
    }
}

/*
SETUP INSTRUCTIONS:

1. SELECT POINT A:
   - Add Component > CheckpointTrigger.cs
   - Checkpoint Type: PointA
   - Show Debug: ✅

2. SELECT POINT B:
   - Add Component > CheckpointTrigger.cs
   - Checkpoint Type: PointB
   - Show Debug: ✅

3. VERIFY COLLIDERS:
   Point A & B must have:
   ├─ Box Collider (or Sphere Collider)
   │  ├─ Is Trigger: ✅ (CRITICAL!)
   │  ├─ Size: (4, 4, 4) minimum
   │  └─ Center: (0, 0, 0)
   │
   └─ Rigidbody (optional but recommended)
      ├─ Is Kinematic: ✅
      └─ Use Gravity: ❌

4. VERIFY PLAYER:
   ├─ Tag: "Player" (CRITICAL!)
   ├─ Has collider (CharacterController counts)
   └─ Not in ignored layer

5. TESTING:
   - Play mode
   - Walk into Point A (green sphere)
   - Console: "✅ Player reached Point A"
   - Timer should start
   - Walk to Point B (red sphere)
   - Console: "✅ Player reached Point B"
   - Level complete screen should show

VISUAL GIZMOS:
- Point A: Green sphere + line
- Point B: Red sphere + line
- When selected: Semi-transparent box showing trigger area

TROUBLESHOOTING:
- If not detecting: Increase collider size to (6, 6, 6)
- Verify player Tag is exactly "Player"
- Check Console for any error messages
- Verify Is Trigger is checked
*/