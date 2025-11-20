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

        // Check if item has visual effects script
        CoinVisualEffects visualEffects = item.GetComponent<CoinVisualEffects>();
        if (visualEffects != null)
        {
            // Let the visual effects script handle destruction
            item.GetComponent<Collider>().enabled = false; // Prevent multiple collections
        }
        else
        {
            // No visual effects, destroy immediately
            Destroy(item);
        }
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

