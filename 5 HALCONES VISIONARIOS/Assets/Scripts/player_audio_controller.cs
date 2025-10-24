using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private bool wasGrounded = true;
    private bool wasMoving = false;
    
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
        
        // Detect jump
        if (playerMovement.IsJumping && wasGrounded)
        {
            AudioManager.Instance.PlayJumpSound();
        }
        
        // Detect landing
        if (isGrounded && !wasGrounded)
        {
            AudioManager.Instance.PlayLandSound();
        }
        
        wasGrounded = isGrounded;
    }
}

/*
SETUP:
1. Add this script to your Player GameObject
2. Make sure the player has PlayerMovement script attached
3. This script will automatically call AudioManager for all player sounds
*/