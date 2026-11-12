using UnityEngine;

/// <summary>
/// Handles proximity collection with 'F' key, triggers pick-up animation, 
/// notifies UI and Level managers, and registers the discovery in the Field Notebook.
/// </summary>
public class CollectibleItem : MonoBehaviour
{
    [Header("Item Info")]
    public string itemName = "Endemic Orchid";
    public float interactionDistance = 3.0f;
    public float destroyDelay = 1.0f; // Delay before complete destruction to match animation duration

    [Header("UI Prompt")]
    public GameObject interactionPromptUI; 

    private Transform playerTransform;
    private Animator playerAnimator;
    private bool isPlayerNearby = false;
    private bool isCollected = false;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) 
        {
            playerTransform = player.transform;
            playerAnimator = player.GetComponent<Animator>(); 
        }

        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
    }

    void Update()
    {
        if (playerTransform == null || isCollected) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactionDistance)
        {
            isPlayerNearby = true;
            if (interactionPromptUI != null && !interactionPromptUI.activeSelf)
            {
                interactionPromptUI.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                CollectItem();
            }
        }
        else
        {
            if (isPlayerNearby)
            {
                isPlayerNearby = false;
                if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
            }
        }
    }

    private void CollectItem()
    {
        isCollected = true;
        Debug.Log("Collected: " + itemName);

        // Trigger the pick-up animation on the player character
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("PickUp");
        }

        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);

        // --- INTEGRACIÓN CON LA LIBRETA DE CAMPO (GABRIEL) ---
        if (FieldDiscovery.Instance != null)
        {
            FieldDiscovery.Instance.AddDiscovery(
                "orquidea_" + itemName.ToLower().Replace(" ", "_"), 
                itemName, 
                "Especie vegetal endémica registrada durante la exploración en el ecosistema.", 
                FieldDiscoveryCategory.Especie
            );
        }

        // Notify UI Manager to show notification feedback
        if (UIManager.Instance != null)
        {
            UIManager.Instance.NotifyNewItemCollected();
        }

        // Notify Level Manager to update progress
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.AddCollectible();
        }

        // Disable mesh and collider immediately for visual feedback, 
        // then destroy the GameObject after the specified delay
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null) mesh.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, destroyDelay);
    }
}