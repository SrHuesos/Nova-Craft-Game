using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages HUD notifications and collection counter.
/// Gabriel's FieldNotebookUI script automatically handles the 'B' key.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public GameObject notificationBadge; // Un pequeño brillo o punto de notificación en el botón

    [Header("HUD Counters")]
    public TMP_Text collectiblesCounterText; // Muestra "1 / 3"

    private int currentCollected = 0;
    private int totalItems = 3;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (notificationBadge != null) notificationBadge.SetActive(false);
        UpdateCounterUI();
    }

    public void NotifyNewItemCollected()
    {
        currentCollected++;
        UpdateCounterUI();

        // Enciende el aviso visual para que el jugador sepa que hay algo nuevo en la libreta
        if (notificationBadge != null)
        {
            notificationBadge.SetActive(true);
        }
    }

    private void UpdateCounterUI()
    {
        if (collectiblesCounterText != null)
        {
            collectiblesCounterText.text = "Species: " + currentCollected + " / " + totalItems;
        }
    }
}