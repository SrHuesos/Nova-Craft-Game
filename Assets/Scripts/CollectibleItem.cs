using UnityEngine;

/// <summary>
/// Handles proximity collection with 'F' key, triggers pick-up animation, 
/// notifies UI and Level managers, and registers the discovery in the Field Notebook.
/// </summary>
public class CollectibleItem : MonoBehaviour
{
    [Header("Item Info")]
    public string itemIdPrefix = "especie";
    public string itemName = "Endemic Plant";
    public string discoveryDescription = "Especie vegetal registrada durante la exploración en el ecosistema.";
    public float interactionDistance = 4.5f;
    public float destroyDelay = 1.0f;

    [Header("UI Prompt")]
    public GameObject interactionPromptUI;

    [Header("Audio Settings")]                  // SONIDO
    public AudioSource audioSource;             // SONIDO
    public AudioClip collectSound;              // SONIDO

    private Transform playerTransform;
    private Animator playerAnimator;
    private bool isPlayerNearby = false;
    private bool isCollected = false;
    private bool loggedMissingPlayer = false;

    void Start()
    {
        TryFindPlayer();
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
    }

    private void TryFindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerAnimator = player.GetComponentInChildren<Animator>();
            if (playerAnimator == null)
            {
                playerAnimator = player.GetComponent<Animator>();
            }
            loggedMissingPlayer = false;
        }
        else if (!loggedMissingPlayer)
        {
            loggedMissingPlayer = true;
            Debug.LogWarning("[CollectibleItem] No se encontró ningún objeto con Tag 'Player' en la escena.");
        }
    }

    void Update()
    {
        if (isCollected) return;

        if (playerTransform == null)
        {
            TryFindPlayer();
            if (playerTransform == null) return;
        }

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
                Debug.Log("[CollectibleItem] Tecla F detectada en " + itemName + " (distancia: " + distance.ToString("F2") + ")");
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
        Debug.Log("[CollectibleItem] Recolectado: " + itemName);

        if (audioSource != null && collectSound != null)    // SONIDO
        {                                                   // SONIDO
            audioSource.PlayOneShot(collectSound);          // SONIDO
        }                                                   // SONIDO

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("PickUp");
        }

        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);

        if (FieldDiscovery.Instance != null)
        {
            string cleanPrefix = string.IsNullOrEmpty(itemIdPrefix) ? "especie" : itemIdPrefix.ToLower().Replace(" ", "_");
            string cleanName = itemName.ToLower().Replace(" ", "_");
            FieldDiscovery.Instance.AddDiscovery(
                cleanPrefix + "_" + cleanName,
                itemName,
                discoveryDescription,
                FieldDiscoveryCategory.Especie
            );
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.NotifyNewItemCollected();
        }
        else
        {
            Debug.LogWarning("[CollectibleItem] UIManager.Instance es null en esta escena.");
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.AddCollectible();
        }
        else
        {
            Debug.LogWarning("[CollectibleItem] LevelManager.Instance es null en esta escena.");
        }

        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer m in meshes)
        {
            m.enabled = false;
        }

        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (Collider c in cols)
        {
            c.enabled = false;
        }

        Destroy(gameObject, destroyDelay);
    }
}