using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Panels - Assign from Canvas children")]
    public GameObject mainMenuPanel;
    public GameObject characterSelectionPanel;
    public GameObject settingsPanel;
    public GameObject tutorialPanel;

    [Header("Scene Settings")]
    public string gameSceneName = "MainScene";

    void Start()
    {
        // Show main menu by default
        ShowMainMenu();

        // Ensure cursor is visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ===== MAIN MENU BUTTONS =====
    public void OnQuickPlay()
    {
        LoadGame();
    }

    public void OnCharacters()
    {
        ShowPanel(characterSelectionPanel);
    }

    public void OnTutorial()
    {
        ShowPanel(tutorialPanel);
    }

    public void OnSettings()
    {
        ShowPanel(settingsPanel);
    }

    public void OnExit()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ===== NAVIGATION =====
    public void ShowMainMenu()
    {
        ShowPanel(mainMenuPanel);
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    // ===== PANEL MANAGEMENT =====
    void ShowPanel(GameObject panelToShow)
    {
        // Hide all panels
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (characterSelectionPanel != null) characterSelectionPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);

        // Show requested panel
        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }

    // ===== SCENE LOADING =====
    public void LoadGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadMainMenuScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}