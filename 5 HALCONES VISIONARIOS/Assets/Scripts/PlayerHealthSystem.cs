using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerHealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;
    public bool isInvulnerable = false;
    public float invulnerabilityDuration = 1.5f;

    [Header("Death Settings")]
    public float deathYPosition = -10f; // Fall death limit
    public float respawnDelay = 2f;
    public bool respawnOnDeath = true;
    public Vector3 respawnPosition;

    [Header("Damage Settings")]
    public LayerMask damageLayer;
    public string damageTag = "Damage";

    [Header("Events")]
    public UnityEvent<int> OnHealthChanged;
    public UnityEvent OnPlayerDamaged;
    public UnityEvent OnPlayerDeath;
    public UnityEvent OnPlayerRespawn;

    [Header("Animation")]
    private Animator animator;
    private readonly string PARAM_DAMAGE = "TakeDamage";
    private readonly string PARAM_DEATH = "Death";

    private bool isDead = false;
    private float invulnerabilityTimer = 0f;
    private PlayerMovement playerMovement;
    private CharacterController characterController;

    public bool IsDead { get { return isDead; } }
    public bool IsInvulnerable { get { return isInvulnerable; } }

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();

        // Save initial position as respawn point
        if (respawnPosition == Vector3.zero)
        {
            respawnPosition = transform.position;
        }

        OnHealthChanged?.Invoke(currentHealth);
    }

    void Update()
    {
        // Check for fall death
        if (!isDead && transform.position.y < deathYPosition)
        {
            Die("Fell off the map");
        }

        // Update invulnerability timer
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0)
            {
                isInvulnerable = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check for damage objects
        if (!isDead && !isInvulnerable)
        {
            if (other.CompareTag(damageTag) || IsInLayerMask(other.gameObject.layer, damageLayer))
            {
                TakeDamage(1);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Alternative damage detection for non-trigger colliders
        if (!isDead && !isInvulnerable)
        {
            if (collision.gameObject.CompareTag(damageTag) ||
                IsInLayerMask(collision.gameObject.layer, damageLayer))
            {
                TakeDamage(1);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

        // Trigger damage animation
        if (animator != null)
        {
            animator.SetTrigger(PARAM_DAMAGE);
        }

        // Play damage sound
        if (AudioManager.Instance != null)
        {
            // Add damage sound to AudioManager if needed
            // AudioManager.Instance.PlayDamageSound();
        }

        // Activate invulnerability
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;

        // Invoke events
        OnHealthChanged?.Invoke(currentHealth);
        OnPlayerDamaged?.Invoke();

        // Check for death
        if (currentHealth <= 0)
        {
            Die("Health depleted");
        }
    }

    public void Die(string reason = "")
    {
        if (isDead) return;

        isDead = true;

        Debug.Log($"Player died: {reason}");

        // Trigger death animation
        if (animator != null)
        {
            animator.SetTrigger(PARAM_DEATH);
        }

        // Disable movement
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // Play death sound
        if (AudioManager.Instance != null)
        {
            // Add death sound to AudioManager if needed
            // AudioManager.Instance.PlayDeathSound();
        }

        // Invoke death event
        OnPlayerDeath?.Invoke();

        // Handle respawn or game over
        if (respawnOnDeath)
        {
            Invoke(nameof(Respawn), respawnDelay);
        }
        else
        {
            Invoke(nameof(GameOver), respawnDelay);
        }
    }

    void Respawn()
    {
        if (!respawnOnDeath)
        {
            Debug.Log("Respawn disabled. Waiting for restart.");
            return;
        }

        isDead = false;
        currentHealth = maxHealth;

        // Reset position
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.position = respawnPosition;

        if (characterController != null)
        {
            characterController.enabled = true;
        }

        // Re-enable movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Reset animator to idle
        if (animator != null)
        {
            animator.Play("Idle", 0, 0f);
        }

        // Brief invulnerability after respawn
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;

        OnHealthChanged?.Invoke(currentHealth);
        OnPlayerRespawn?.Invoke();

        Debug.Log("Player respawned");
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        OnHealthChanged?.Invoke(currentHealth);

        Debug.Log($"Player healed {amount}. Health: {currentHealth}/{maxHealth}");
    }

    void GameOver()
    {
        Debug.Log("Game Over - waiting for player input");
        // Don't reload automatically
        // Let the GameHUDManager handle restart via button
        // Or wait for manual restart
    }

    // Called when time runs out (from GameManager)
    public void TimeOut()
    {
        Die("Time's up!");
    }

    bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    // Visual feedback for invulnerability (optional)
    void OnDrawGizmos()
    {
        if (isInvulnerable && Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}