using UnityEngine;
using TMPro;

/// <summary>
/// Manages HUD notifications and collection counter.
/// Gabriel's FieldNotebookUI script automatically handles the 'B' key.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public GameObject notificationBadge;

    [Header("HUD Counters")]
    public TMP_Text collectiblesCounterText;

    private int currentCollected = 0;
    private int totalItems = 3;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (notificationBadge != null) notificationBadge.SetActive(false);
        UpdateCounterUI();
    }

    public void ResetCounter(int newTotalItems)
    {
        currentCollected = 0;
        totalItems = Mathf.Max(1, newTotalItems);
        if (notificationBadge != null) notificationBadge.SetActive(false);
        UpdateCounterUI();
    }

    public void NotifyNewItemCollected()
    {
        currentCollected++;
        UpdateCounterUI();

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