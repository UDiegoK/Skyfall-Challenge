using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;
    
    // Animator parameter names
    private readonly string PARAM_SPEED = "Speed";
    private readonly string PARAM_GROUNDED = "IsGrounded";
    private readonly string PARAM_JUMP = "Jump";
    private readonly string PARAM_RUNNING = "IsRunning";
    
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
        
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on " + gameObject.name);
        }
    }
    
    void Update()
    {
        if (animator == null || playerMovement == null) return;
        
        // Calculate speed magnitude for blend between idle and walk
        float speedMagnitude = playerMovement.Velocity.magnitude;
        animator.SetFloat(PARAM_SPEED, speedMagnitude);
        
        // Set grounded state
        animator.SetBool(PARAM_GROUNDED, playerMovement.IsGrounded);
        
        // Set running state
        animator.SetBool(PARAM_RUNNING, playerMovement.IsRunning);
        
        // Trigger jump animation
        if (playerMovement.IsJumping)
        {
            animator.SetTrigger(PARAM_JUMP);
        }
    }
}

/* 
ANIMATOR SETUP INSTRUCTIONS:
1. Create these parameters in your Animator Controller:
   - Speed (Float) - Controls walk/idle blend
   - IsGrounded (Bool) - Detects if player is on ground
   - Jump (Trigger) - Triggers jump animation
   - IsRunning (Bool) - Switches to running animation

2. Recommended Animation States:
   - Idle (Speed = 0)
   - Walk (Speed > 0 && Speed < 0.5)
   - Run (IsRunning = true)
   - Jump (Jump trigger)
   - Fall (IsGrounded = false && vertical velocity < 0)
*/