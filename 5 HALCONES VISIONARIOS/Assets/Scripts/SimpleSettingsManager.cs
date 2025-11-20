using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleSettingsManager : MonoBehaviour
{
    [Header("Sub-Panels")]
    public GameObject audioOptionsPanel;
    public GameObject controlsPanel;
    public GameObject mainSettingsPanel; // The main settings menu with "Audio options" and "Controls" buttons

    [Header("Audio Sliders (Optional)")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    void Start()
    {
        // Show main settings panel by default
        ShowMainSettings();

        // Load saved settings
        LoadAudioSettings();

        // Setup slider listeners
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    // ===== NAVIGATION =====
    public void ShowAudioOptions()
    {
        if (mainSettingsPanel != null) mainSettingsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (audioOptionsPanel != null) audioOptionsPanel.SetActive(true);
    }

    public void ShowControls()
    {
        if (mainSettingsPanel != null) mainSettingsPanel.SetActive(false);
        if (audioOptionsPanel != null) audioOptionsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(true);
    }

    public void ShowMainSettings()
    {
        if (audioOptionsPanel != null) audioOptionsPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (mainSettingsPanel != null) mainSettingsPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        MenuManager menuManager = FindObjectOfType<MenuManager>();
        if (menuManager != null)
        {
            menuManager.BackToMainMenu();
        }
    }

    // ===== AUDIO SETTINGS =====
    public void SetMasterVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(volume);
            AudioManager.Instance.SetMusicVolume(volume);
            AudioManager.Instance.SetFootstepVolume(volume);
        }

        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }

        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(volume);
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    void LoadAudioSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (masterVolumeSlider != null)
            masterVolumeSlider.value = masterVolume;

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = musicVolume;

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfxVolume;
    }

    public void ApplySettings()
    {
        PlayerPrefs.Save();
        Debug.Log("Settings saved!");
    }
}
