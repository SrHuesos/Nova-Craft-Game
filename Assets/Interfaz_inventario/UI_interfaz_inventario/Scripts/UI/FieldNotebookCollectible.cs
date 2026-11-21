using UnityEngine;

public class FieldNotebookCollectible : MonoBehaviour
{
    [Header("Discovery Information")]
    [SerializeField] private string discoveryId = "especie_orquidea_001";
    [SerializeField] private string discoveryTitle = "Orquídea de prueba";
    [TextArea(2, 5)]
    [SerializeField] private string description =
        "Una especie vegetal registrada durante la exploración.";
    [SerializeField] private FieldDiscoveryCategory category =
        FieldDiscoveryCategory.Especie;

    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 3f;

    private Transform player;
    private bool collected;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogWarning(
                "No se encontró un objeto con el tag Player para recoger: "
                + discoveryTitle);
    }

    private void Update()
    {
        if (collected || player == null)
            return;

        float distance = Vector3.Distance(
            transform.position, player.position);

        if (distance <= interactionDistance &&
            Input.GetKeyDown(KeyCode.F))
        {
            Collect();
        }
    }

    private void Collect()
    {
        FieldDiscovery manager = FieldDiscovery.Instance;

        if (manager == null)
        {
            Debug.LogWarning(
                "No se encontró FieldDiscovery. No se registró el hallazgo.");
            return;
        }

        bool added = manager.AddDiscovery(
            discoveryId, discoveryTitle, description, category);

        if (added)
        {
            collected = true;
            Debug.Log("Hallazgo añadido a la libreta: " + discoveryTitle);
            Destroy(gameObject);
        }
    }
}
