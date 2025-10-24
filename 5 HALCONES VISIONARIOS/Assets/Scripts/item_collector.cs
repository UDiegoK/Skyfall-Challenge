using UnityEngine;
using UnityEngine.Events;

public class ItemCollector : MonoBehaviour
{
    [Header("Collection Mode")]
    [Tooltip("If true, items are collected on collision. If false, press a key to collect.")]
    public bool autoCollectOnTrigger = true;
    public KeyCode pickupKey = KeyCode.E;
    
    [Header("Manual Collection Settings")]
    public float collectionRadius = 2f;
    public LayerMask itemLayer;
    
    [Header("UI Feedback")]
    public bool showDebugMessages = true;
    
    [Header("Events")]
    public UnityEvent<GameObject> OnItemCollected;
    public UnityEvent<int> OnCoinCollected;
    
    private int itemsCollected = 0;
    private int coinsCollected = 0;
    private GameObject nearestItem;
    
    public int ItemsCollected { get { return itemsCollected; } }
    public int CoinsCollected { get { return coinsCollected; } }
    
    void Update()
    {
        // Only use manual collection if autoCollect is disabled
        if (!autoCollectOnTrigger)
        {
            DetectNearbyItems();
            
            if (Input.GetKeyDown(pickupKey) && nearestItem != null)
            {
                CollectItem(nearestItem);
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Auto collect on trigger
        if (autoCollectOnTrigger)
        {
            Collectible collectible = other.GetComponent<Collectible>();
            
            if (collectible != null)
            {
                CollectItem(other.gameObject);
            }
        }
    }
    
    void DetectNearbyItems()
    {
        Collider[] itemsInRange = Physics.OverlapSphere(transform.position, collectionRadius, itemLayer);
        
        nearestItem = null;
        float closestDistance = collectionRadius;
        
        foreach (Collider item in itemsInRange)
        {
            float distance = Vector3.Distance(transform.position, item.transform.position);
            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestItem = item.gameObject;
            }
        }
    }
    
    void CollectItem(GameObject item)
    {
        if (item == null) return;
        
        // Get collectible component
        Collectible collectible = item.GetComponent<Collectible>();
        
        if (collectible != null)
        {
            collectible.Collect();
            
            // Check if it's a coin
            if (collectible.itemType == Collectible.ItemType.Coin)
            {
                coinsCollected += collectible.coinValue;
                OnCoinCollected?.Invoke(coinsCollected);
                
                if (showDebugMessages)
                {
                    Debug.Log("Coin collected! Value: " + collectible.coinValue + " | Total coins: " + coinsCollected);
                }
            }
        }
        
        itemsCollected++;
        
        if (showDebugMessages)
        {
            Debug.Log("Item collected: " + item.name + " | Total items: " + itemsCollected);
        }
        
        // Play collection sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollectSound();
        }
        
        // Invoke event
        OnItemCollected?.Invoke(item);
        
        // Destroy the item
        Destroy(item);
    }
    
    public void ResetCollector()
    {
        itemsCollected = 0;
        coinsCollected = 0;
    }
    
    void OnDrawGizmosSelected()
    {
        if (!autoCollectOnTrigger)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, collectionRadius);
            
            if (nearestItem != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nearestItem.transform.position);
            }
        }
    }
}

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
        
        // Add particle effects here if needed
        Debug.Log(itemName + " collected! Points: " + points);
    }
}

/*
SETUP INSTRUCTIONS FOR COINS:

1. Create a coin prefab (3D object like Cylinder or Sphere)
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