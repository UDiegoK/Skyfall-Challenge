using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUDManager : MonoBehaviour
{
    [Header("Timer Display")]
    public TextMeshProUGUI timerText;
    public Slider timeSlider;
    public float maxTime = 60f; // 1 minute
    private float currentTime;

    [Header("Progress Bar")]
    public Slider progressSlider;
    public TextMeshProUGUI progressPercentText;
    public Transform pointA;
    public Transform pointB;
    private Transform player;

    [Header("Health Display")]
    public Image[] healthHearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Coins Display")]
    public TextMeshProUGUI coinsText;
    private int currentCoins = 0;

    [Header("References")]
    public PlayerHealthSystem playerHealth;
    public ItemCollector itemCollector;
    public CheckpointTimer checkpointTimer;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public GameObject winPanel;
    public TextMeshProUGUI winText;

    private bool timerRunning = false;
    private bool gameEnded = false;

    void Start()
    {
        // Initialize timer
        currentTime = maxTime;

        // Setup initial slider values
        if (timeSlider != null)
        {
            timeSlider.maxValue = maxTime;
            timeSlider.value = maxTime; // Start full
        }

        // Initialize progress bar at 0
        if (progressSlider != null)
        {
            progressSlider.minValue = 0;
            progressSlider.maxValue = 1;
            progressSlider.value = 0;
        }

        // Hide game over panels
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (winPanel != null)
            winPanel.SetActive(false);

        // Find player
        FindPlayer();

        // Find components if not assigned
        if (playerHealth == null && player != null)
            playerHealth = player.GetComponent<PlayerHealthSystem>();

        if (itemCollector == null && player != null)
            itemCollector = player.GetComponent<ItemCollector>();

        if (checkpointTimer == null)
            checkpointTimer = FindObjectOfType<CheckpointTimer>();

        // Subscribe to events
        SetupEvents();

        // Update displays
        UpdateTimerDisplay();
        UpdateHealthDisplay();
        UpdateCoinsDisplay();
    }

    void FindPlayer()
    {
        // Try to find active player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && playerObj.activeInHierarchy)
        {
            player = playerObj.transform;
        }
        else
        {
            // Find from PlayerSpawner
            PlayerSpawner spawner = FindObjectOfType<PlayerSpawner>();
            if (spawner != null)
            {
                GameObject activePlayer = spawner.GetActivePlayer();
                if (activePlayer != null)
                {
                    player = activePlayer.transform;
                }
            }
        }

        if (player == null)
        {
            Debug.LogWarning("Player not found! HUD may not work correctly.");
        }
    }

    void Update()
    {
        if (gameEnded) return;

        // Re-find player if lost
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            FindPlayer();
        }

        // Update timer
        if (timerRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                TimeUp();
            }

            UpdateTimerDisplay();
        }

        // Update progress bar
        UpdateProgressBar();
    }

    void SetupEvents()
    {
        // Health events
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthDisplay);
            playerHealth.OnPlayerDeath.AddListener(OnPlayerDeath);
        }

        // Coin collection events
        if (itemCollector != null)
        {
            itemCollector.OnCoinCollected.AddListener(UpdateCoinsDisplay);
        }

        // Checkpoint events
        if (checkpointTimer != null)
        {
            checkpointTimer.OnTimerStart.AddListener(StartTimer);
            checkpointTimer.OnCheckpointReached.AddListener(OnLevelComplete);
        }
    }

    // ===== TIMER =====
    public void StartTimer()
    {
        timerRunning = true;
        currentTime = maxTime;
        Debug.Log("Timer started! Max time: " + maxTime);
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        if (timerText != null)
        {
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Change color when low time
            if (currentTime <= 10f)
            {
                timerText.color = Color.red;
            }
            else if (currentTime <= 30f)
            {
                timerText.color = Color.yellow;
            }
            else
            {
                timerText.color = Color.white;
            }
        }

        if (timeSlider != null)
        {
            timeSlider.value = currentTime;
        }
    }

    void TimeUp()
    {
        if (gameEnded) return; // Prevent multiple calls

        timerRunning = false;
        gameEnded = true;

        Debug.Log("Time's up! Calling player death...");

        if (playerHealth != null)
        {
            playerHealth.TimeOut();
        }

        // Show game over even if player health is null
        Invoke(nameof(ShowTimeUpScreen), 1f);
    }

    void ShowTimeUpScreen()
    {
        ShowGameOver("Time's Up!");
    }

    // ===== PROGRESS BAR =====
    void UpdateProgressBar()
    {
        if (progressSlider == null || player == null || pointA == null || pointB == null)
            return;

        // Calculate progress based on distance from A to B
        float totalDistance = Vector3.Distance(pointA.position, pointB.position);

        if (totalDistance <= 0)
        {
            Debug.LogWarning("Point A and Point B are at the same position!");
            return;
        }

        float distanceFromA = Vector3.Distance(pointA.position, player.position);
        float progress = Mathf.Clamp01(distanceFromA / totalDistance);

        progressSlider.value = progress;

        if (progressPercentText != null)
        {
            progressPercentText.text = Mathf.RoundToInt(progress * 100f) + "%";
        }
    }

    // ===== HEALTH =====
    void UpdateHealthDisplay(int health)
    {
        UpdateHealthDisplay();
    }

    void UpdateHealthDisplay()
    {
        if (healthHearts == null || healthHearts.Length == 0) return;
        if (playerHealth == null) return;

        for (int i = 0; i < healthHearts.Length; i++)
        {
            if (i < playerHealth.currentHealth)
            {
                healthHearts[i].sprite = fullHeart;
                healthHearts[i].enabled = true;
            }
            else if (i < playerHealth.maxHealth)
            {
                healthHearts[i].sprite = emptyHeart;
                healthHearts[i].enabled = true;
            }
            else
            {
                healthHearts[i].enabled = false;
            }
        }
    }

    // ===== COINS =====
    void UpdateCoinsDisplay(int coins)
    {
        currentCoins = coins;
        UpdateCoinsDisplay();
    }

    void UpdateCoinsDisplay()
    {
        if (coinsText != null)
        {
            if (itemCollector != null)
            {
                coinsText.text = itemCollector.CoinsCollected.ToString();
            }
            else
            {
                coinsText.text = currentCoins.ToString();
            }
        }
    }

    // ===== GAME OVER =====
    void OnPlayerDeath()
    {
        if (!gameEnded)
        {
            gameEnded = true;
            timerRunning = false;

            // Don't destroy player, just show game over
            Invoke(nameof(ShowDeathScreen), 1.5f);
        }
    }

    void ShowDeathScreen()
    {
        ShowGameOver("You Died!");
    }

    void OnLevelComplete(float finalTime)
    {
        if (gameEnded) return;

        gameEnded = true;
        timerRunning = false;

        Debug.Log("Level Complete! Time: " + finalTime);

        int minutes = Mathf.FloorToInt(finalTime / 60f);
        int seconds = Mathf.FloorToInt(finalTime % 60f);
        int milliseconds = Mathf.FloorToInt((finalTime * 100f) % 100f);

        string timeString = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        string message = $"Level Complete!\nTime: {timeString}\nCoins: {currentCoins}";

        ShowWin(message);
    }

    void ShowGameOver(string message)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverText != null)
        {
            gameOverText.text = message;
        }

        Debug.Log("Game Over: " + message);
    }

    void ShowWin(string message)
    {
        // Use win panel if exists, otherwise use game over panel
        if (winPanel != null)
        {
            winPanel.SetActive(true);

            if (winText != null)
            {
                winText.text = message;
            }
        }
        else if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (gameOverText != null)
            {
                gameOverText.text = message;
            }
        }

        Debug.Log("Victory: " + message);
    }

    // ===== PUBLIC METHODS =====
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}

/*
FIXES APPLIED:

1. TIMER ISSUE:
   - timeSlider now starts at maxValue (full)
   - Properly initializes in Start()
   - Updates correctly during countdown

2. PLAYER DELETION ISSUE:
   - Removed player destruction from death
   - Game over shows after delay
   - Player object stays in scene
   - Buttons are interactable

3. WIN PANEL ISSUE:
   - Added separate winPanel (optional)
   - OnLevelComplete properly triggers
   - Shows completion message with stats
   - Can use gameOverPanel if winPanel not assigned

4. PLAYER FINDING:
   - Better player detection
   - Works with PlayerSpawner
   - Re-finds player if lost

SETUP:
- Optionally create separate Win Panel
- Assign to Win Panel field
- Or use same panel for both (gameOverPanel)
*/