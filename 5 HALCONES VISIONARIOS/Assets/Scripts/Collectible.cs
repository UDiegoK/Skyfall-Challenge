using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Collectible script for items and coins
public class Collectible : MonoBehaviour
{
    public enum ItemType
    {
        Coin,
        Item,
        PowerUp
    }

    [Header("Item Properties")]
    public ItemType itemType = ItemType.Coin;
    public string itemName = "Coin";
    public int coinValue = 1;
    public int points = 10;

    [Header("Visual Feedback")]
    public bool rotateItem = true;
    public float rotationSpeed = 50f;
    public bool bobUpDown = false;
    public float bobHeight = 0.3f;
    public float bobSpeed = 2f;

    [Header("Audio")]
    public AudioClip collectSound;

    private Vector3 startPosition;
    private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (rotateItem)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        if (bobUpDown)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    public void Collect()
    {
        // Play sound effect
        if (collectSound != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Trigger visual effects
        CoinVisualEffects visualEffects = GetComponent<CoinVisualEffects>();
        if (visualEffects != null)
        {
            visualEffects.PlayCollectionEffect();
        }

        Debug.Log(itemName + " collected! Points: " + points);
    }
}

/*
SETUP INSTRUCTIONS FOR COINS:

1. Create a coin prefab (3D object)
2. Add a Collider component and check "Is Trigger"
3. Add the Collectible script to the coin
4. Set ItemType to "Coin"
5. Set coinValue (1 for regular coins, 5 for gold coins, etc.)
6. Enable "Rotate Item" and "Bob Up Down" for nice visual effect

7. On your Player:
   - Add the ItemCollector script
   - Check "Auto Collect On Trigger" 
   - The player will now collect coins automatically on contact!

8. Make sure your player has a Collider (preferably CharacterController or Capsule Collider)
*/