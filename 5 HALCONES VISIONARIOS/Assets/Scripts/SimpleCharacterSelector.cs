using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleCharacterSelector : MonoBehaviour
{
    [Header("Character Buttons")]
    public Button mrWhiteButton;
    public Button mrPurpleButton;
    public Button mrCoffeeButton;
    public Button mrGreenButton;

    [Header("Play Button")]
    public Button playButton;

    [Header("Visual Feedback (Optional)")]
    public GameObject[] selectionFrames; // Optional: highlight frames for each character

    private int selectedCharacter = 0; // 0=White, 1=Purple, 2=Coffee, 3=Green

    void Start()
    {
        // Setup character selection buttons
        if (mrWhiteButton != null)
            mrWhiteButton.onClick.AddListener(() => SelectCharacter(0));

        if (mrPurpleButton != null)
            mrPurpleButton.onClick.AddListener(() => SelectCharacter(1));

        if (mrCoffeeButton != null)
            mrCoffeeButton.onClick.AddListener(() => SelectCharacter(2));

        if (mrGreenButton != null)
            mrGreenButton.onClick.AddListener(() => SelectCharacter(3));

        // Setup play button
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);

        // Load saved character or default to first
        selectedCharacter = PlayerPrefs.GetInt("SelectedCharacter", 0);
        UpdateVisualFeedback();
    }

    public void SelectCharacter(int characterIndex)
    {
        selectedCharacter = characterIndex;
        UpdateVisualFeedback();

        string[] characterNames = { "Mr. White", "Mr. Purple", "Mr. Coffee", "Mr. Green" };
        Debug.Log("Character selected: " + characterNames[characterIndex]);
    }

    void UpdateVisualFeedback()
    {
        // Hide all selection frames
        if (selectionFrames != null)
        {
            for (int i = 0; i < selectionFrames.Length; i++)
            {
                if (selectionFrames[i] != null)
                {
                    selectionFrames[i].SetActive(i == selectedCharacter);
                }
            }
        }
    }

    void OnPlayButtonClicked()
    {
        // Save selected character
        PlayerPrefs.SetInt("SelectedCharacter", selectedCharacter);
        PlayerPrefs.Save();

        // Start game
        MenuManager menuManager = FindObjectOfType<MenuManager>();
        if (menuManager != null)
        {
            menuManager.LoadGame();
        }
        else
        {
            Debug.LogError("MenuManager not found!");
        }
    }

    // Public method to get selected character (called from game scene)
    public static int GetSelectedCharacter()
    {
        return PlayerPrefs.GetInt("SelectedCharacter", 0);
    }
}

/*
SETUP INSTRUCTIONS:

1. ADD SCRIPT:
   - Select "Character selection panel"
   - Add Component > SimpleCharacterSelector.cs

2. ASSIGN BUTTONS:
   Inspector > SimpleCharacterSelector:
   - Mr White Button: [arrastra el botón de Mr. White]
   - Mr Purple Button: [arrastra el botón de Mr. Purple]
   - Mr Coffee Button: [arrastra el botón de Mr. Coffee]
   - Mr Green Button: [arrastra el botón de Mr. Green]
   - Play Button: [arrastra el botón "Play"]

3. OPTIONAL - VISUAL FEEDBACK:
   Si quieres mostrar cuál está seleccionado:
   - Crea un marco/borde alrededor de cada personaje
   - Asigna estos marcos en "Selection Frames" (array de 4)
   - El script los activará/desactivará automáticamente

4. NO NEED TO CONFIGURE ONCLICK:
   El script maneja todo automáticamente.

5. TO USE IN GAME:
   En tu escena de juego, para saber qué personaje fue seleccionado:
   
   int character = SimpleCharacterSelector.GetSelectedCharacter();
   // 0 = Mr. White
   // 1 = Mr. Purple
   // 2 = Mr. Coffee
   // 3 = Mr. Green
*/