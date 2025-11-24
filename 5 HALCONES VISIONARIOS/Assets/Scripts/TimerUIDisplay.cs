using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerUIDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI itemCountText;
    public TextMeshProUGUI finalTimeText;
    public GameObject finalTimePanel;
    
    [Header("Script References")]
    public CheckpointTimer checkpointTimer;
    public ItemCollector itemCollector;
    
    [Header("Display Settings")]
    public Color normalColor = Color.white;
    public Color completedColor = Color.green;
    
    void Start()
    {
        if (finalTimePanel != null)
        {
            finalTimePanel.SetActive(false);
        }
        
        // Subscribe to checkpoint event
        if (checkpointTimer != null)
        {
            checkpointTimer.OnCheckpointReached.AddListener(ShowFinalTime);
        }
    }
    
    void Update()
    {
        UpdateTimerDisplay();
        UpdateItemCountDisplay();
    }
    
    void UpdateTimerDisplay()
    {
        if (timerText != null && checkpointTimer != null)
        {
            timerText.text = "Time: " + checkpointTimer.FormattedTime;
            timerText.color = checkpointTimer.IsTimerRunning ? normalColor : completedColor;
        }
    }
    
    void UpdateItemCountDisplay()
    {
        if (itemCountText != null && itemCollector != null)
        {
            itemCountText.text = "Items: " + itemCollector.ItemsCollected;
        }
    }
    
    void ShowFinalTime(float finalTime)
    {
        if (finalTimePanel != null)
        {
            finalTimePanel.SetActive(true);
        }
        
        if (finalTimeText != null)
        {
            int minutes = Mathf.FloorToInt(finalTime / 60f);
            int seconds = Mathf.FloorToInt(finalTime % 60f);
            int milliseconds = Mathf.FloorToInt((finalTime * 1000f) % 1000f);
            
            finalTimeText.text = string.Format("Final Time: {0:00}:{1:00}:{2:000}", 
                minutes, seconds, milliseconds);
            finalTimeText.color = completedColor;
        }
    }
    
    public void HideFinalTimePanel()
    {
        if (finalTimePanel != null)
        {
            finalTimePanel.SetActive(false);
        }
    }
}

/*
SETUP INSTRUCTIONS:
1. Create a Canvas in your scene (UI > Canvas)
2. Add TextMeshProUGUI elements for:
   - Timer display (top of screen)
   - Item count display (top of screen)
   - Final time panel (center, initially disabled)
3. Assign references in the Inspector
4. Make sure you have TextMeshPro package imported
*/