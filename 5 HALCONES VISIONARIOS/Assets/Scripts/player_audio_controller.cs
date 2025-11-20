using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("Sound Cooldowns")]
    [Tooltip("Minimum time between jump sounds")]
    public float jumpSoundCooldown = 0.3f;

    [Tooltip("Minimum time between land sounds")]
    public float landSoundCooldown = 0.2f;

    private PlayerMovement playerMovement;
    private bool wasGrounded = true;
    private bool wasMoving = false;
    private bool hasPlayedJumpSound = false;

    // Cooldown timers
    private float lastJumpSoundTime = -999f;
    private float lastLandSoundTime = -999f;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (playerMovement == null || AudioManager.Instance == null) return;

        HandleFootstepSounds();
        HandleJumpAndLandSounds();
    }

    void HandleFootstepSounds()
    {
        bool isMoving = playerMovement.Velocity.magnitude > 0.1f;
        bool isGrounded = playerMovement.IsGrounded;
        bool isRunning = playerMovement.IsRunning;

        if (isMoving && isGrounded)
        {
            AudioManager.Instance.PlayFootsteps(isRunning, isGrounded);
            wasMoving = true;
        }
        else
        {
            if (wasMoving)
            {
                AudioManager.Instance.StopFootsteps();
                wasMoving = false;
            }
        }
    }

    void HandleJumpAndLandSounds()
    {
        bool isGrounded = playerMovement.IsGrounded;

        // Detect jump - only play once per jump
        if (playerMovement.IsJumping && wasGrounded && !hasPlayedJumpSound)
        {
            // Check cooldown
            if (Time.time >= lastJumpSoundTime + jumpSoundCooldown)
            {
                AudioManager.Instance.PlayJumpSound();
                lastJumpSoundTime = Time.time;
                hasPlayedJumpSound = true;
            }
        }

        // Reset jump sound flag when grounded
        if (isGrounded)
        {
            hasPlayedJumpSound = false;
        }

        // Detect landing - with cooldown protection
        if (isGrounded && !wasGrounded)
        {
            // Check cooldown to prevent rapid fire sounds
            if (Time.time >= lastLandSoundTime + landSoundCooldown)
            {
                AudioManager.Instance.PlayLandSound();
                lastLandSoundTime = Time.time;
            }
        }

        wasGrounded = isGrounded;
    }
}

/*
IMPROVEMENTS:
- Added cooldown timers to prevent sound spam
- Jump sound only plays once per jump (hasPlayedJumpSound flag)
- Landing sound has cooldown protection
- Prevents double sounds when colliding with edges
- Configurable cooldown times in Inspector

SETTINGS GUIDE:
- Jump Sound Cooldown: 0.3s (prevents double jumps)
- Land Sound Cooldown: 0.2s (prevents edge collision spam)

Adjust these values if sounds still overlap or feel delayed.
*/