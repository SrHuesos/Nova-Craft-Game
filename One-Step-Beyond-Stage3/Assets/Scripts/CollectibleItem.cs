using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles proximity-based interaction with biodiversity collectibles using keyboard input 'F'.
/// </summary>
public class CollectibleItem : MonoBehaviour
{
    [Header("Item Configuration")]
    public string itemName = "Endemic Orchid";
    public float interactionDistance = 3.0f; // Max distance to interact

    [Header("UI Feedback")]
    public GameObject interactionPromptUI; // Drag your Canvas UI text/panel here

    private Transform playerTransform;
    private bool isPlayerNearby = false;

    void Start()
    {
        // Automatically find the player object by its tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Ensure the UI prompt is hidden at the beginning
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Calculate distance between player and this collectible item
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Check if player is within the interaction range
        if (distance <= interactionDistance)
        {
            isPlayerNearby = true;

            // Show the UI prompt on screen
            if (interactionPromptUI != null && !interactionPromptUI.activeSelf)
            {
                interactionPromptUI.SetActive(true);
            }

            // Wait for player to press the 'F' key
            if (Input.GetKeyDown(KeyCode.F))
            {
                CollectItem();
            }
        }
        else
        {
            // If player moves away, hide the UI prompt
            if (isPlayerNearby)
            {
                isPlayerNearby = false;

                if (interactionPromptUI != null && interactionPromptUI.activeSelf)
                {
                    interactionPromptUI.SetActive(false);
                }
            }
        }
    }

    private void CollectItem()
    {
        Debug.Log("Biodiversity discovered and collected: " + itemName);

        // Notify the level manager
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.AddCollectible();
        }

        // Hide UI prompt before destroying the object
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }

        // Remove the collectible from the ecosystem scene
        Destroy(gameObject);
    }
}