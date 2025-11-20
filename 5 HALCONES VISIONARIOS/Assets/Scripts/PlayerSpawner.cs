using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Player Characters - All in Scene")]
    public GameObject mrWhitePlayer;
    public GameObject mrPurplePlayer;
    public GameObject mrCoffeePlayer;
    public GameObject mrGreenPlayer;

    [Header("Spawn Settings")]
    public Vector3 spawnPosition = Vector3.zero;
    public bool useSpawnPosition = false;

    [Header("Camera Assignment")]
    public Camera mainCamera;
    public bool assignCameraAutomatically = true;

    private GameObject activePlayer;

    void Start()
    {
        // Get selected character from menu
        int selectedCharacter = SimpleCharacterSelector.GetSelectedCharacter();

        // Activate the selected character
        ActivateCharacter(selectedCharacter);

        // Setup camera to follow active player
        if (assignCameraAutomatically)
        {
            SetupCamera();
        }
    }

    void ActivateCharacter(int characterIndex)
    {
        // Deactivate all characters first
        if (mrWhitePlayer != null) mrWhitePlayer.SetActive(false);
        if (mrPurplePlayer != null) mrPurplePlayer.SetActive(false);
        if (mrCoffeePlayer != null) mrCoffeePlayer.SetActive(false);
        if (mrGreenPlayer != null) mrGreenPlayer.SetActive(false);

        // Activate selected character
        switch (characterIndex)
        {
            case 0: // Mr. White
                if (mrWhitePlayer != null)
                {
                    mrWhitePlayer.SetActive(true);
                    activePlayer = mrWhitePlayer;
                    Debug.Log("Mr. White activated!");
                }
                break;

            case 1: // Mr. Purple
                if (mrPurplePlayer != null)
                {
                    mrPurplePlayer.SetActive(true);
                    activePlayer = mrPurplePlayer;
                    Debug.Log("Mr. Purple activated!");
                }
                break;

            case 2: // Mr. Coffee
                if (mrCoffeePlayer != null)
                {
                    mrCoffeePlayer.SetActive(true);
                    activePlayer = mrCoffeePlayer;
                    Debug.Log("Mr. Coffee activated!");
                }
                break;

            case 3: // Mr. Green
                if (mrGreenPlayer != null)
                {
                    mrGreenPlayer.SetActive(true);
                    activePlayer = mrGreenPlayer;
                    Debug.Log("Mr. Green activated!");
                }
                break;

            default:
                Debug.LogWarning("Invalid character index: " + characterIndex);
                // Activate first character as fallback
                if (mrWhitePlayer != null)
                {
                    mrWhitePlayer.SetActive(true);
                    activePlayer = mrWhitePlayer;
                }
                break;
        }

        // Position player at spawn point if enabled
        if (useSpawnPosition && activePlayer != null)
        {
            activePlayer.transform.position = spawnPosition;
        }
    }

    void SetupCamera()
    {
        if (activePlayer == null) return;

        // Find main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera not found!");
            return;
        }

        // Look for camera follow script (Cinemachine, custom, etc.)
        // Option 1: Simple parent to player
        Transform cameraHolder = activePlayer.transform.Find("CameraHolder");
        if (cameraHolder != null)
        {
            mainCamera.transform.parent = cameraHolder;
            mainCamera.transform.localPosition = new Vector3(0, 0, -5);
            mainCamera.transform.localRotation = Quaternion.identity;
        }

        // Option 2: Cinemachine (if you have it)
#if CINEMACHINE
        var vcam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (vcam != null)
        {
            vcam.Follow = activePlayer.transform;
            vcam.LookAt = activePlayer.transform;
        }
#endif
    }

    // Public method to get current active player
    public GameObject GetActivePlayer()
    {
        return activePlayer;
    }

    // Visualize spawn position in editor
    void OnDrawGizmos()
    {
        if (useSpawnPosition)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPosition, 1f);
            Gizmos.DrawLine(spawnPosition, spawnPosition + Vector3.up * 2f);
        }
    }
}