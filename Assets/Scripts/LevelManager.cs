using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Flow Settings")]
    public int totalCollectibles = 3;
    public string nextSceneName = "Humedal";
    public bool isFinalLevel = false;

    [Header("Exit Gate & Minimap Indicator")]
    public GameObject exitGate;
    public GameObject exitMinimapIcon;
    public bool hideExitIconOnStart = true;

    [Header("Mission UI")]
    public TMP_Text titleMissionText;
    public TMP_Text descriptionMissionText;

    [Header("Final Victory UI (Final Level Only)")]
    public GameObject victoryPanel;
    public TMP_Text victorySummaryText;

    private int collectedCount = 0;
    private int totalSessionCollected = 0;
    private bool levelCompleted = false;

    private void Awake()
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

    private void Start()
    {
        ResetLevelState();
    }

    public void ResetLevelState()
    {
        collectedCount = 0;
        levelCompleted = false;
        Time.timeScale = 1f;

        if (titleMissionText != null)
        {
            titleMissionText.text = "MISIÓN ACTUAL";
        }

        if (exitGate != null)
        {
            exitGate.SetActive(false);
        }

        if (exitMinimapIcon != null)
        {
            exitMinimapIcon.SetActive(!hideExitIconOnStart);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ResetCounter(totalCollectibles);
        }

        UpdateMissionUI();
    }

    public void AddCollectible()
    {
        if (levelCompleted) return;

        collectedCount++;
        totalSessionCollected++;
        UpdateMissionUI();

        if (collectedCount >= totalCollectibles)
        {
            CompleteCollectionObjective();
        }
    }

    private void UpdateMissionUI()
    {
        if (descriptionMissionText == null) return;

        if (collectedCount < totalCollectibles)
        {
            descriptionMissionText.text = "Busca las plantas (" + collectedCount + "/" + totalCollectibles + ")";
        }
        else if (!isFinalLevel)
        {
            descriptionMissionText.text = "¡Objetos completos! Ve a la puerta de salida.";
        }
        else
        {
            descriptionMissionText.text = "¡Especies del humedal completas! Misión finalizada.";
        }
    }

    private void CompleteCollectionObjective()
    {
        levelCompleted = true;

        if (!isFinalLevel)
        {
            if (exitGate != null)
            {
                exitGate.SetActive(true);
            }

            if (exitMinimapIcon != null)
            {
                exitMinimapIcon.SetActive(true);
            }
        }
        else
        {
            if (exitMinimapIcon != null)
            {
                exitMinimapIcon.SetActive(false);
            }

            if (exitGate != null)
            {
                exitGate.SetActive(false);
            }

            ShowVictoryPanel();
        }
    }

    public void TriggerExitGate()
    {
        if (!levelCompleted || collectedCount < totalCollectibles)
        {
            Debug.Log("La puerta de salida está bloqueada: completa la recolección primero.");
            return;
        }

        if (isFinalLevel)
        {
            ShowVictoryPanel();
            return;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void ShowVictoryPanel()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        if (victorySummaryText != null)
        {
            victorySummaryText.text = "¡Felicidades! Completaste la exploración.\nPlantas recolectadas en este nivel: " + collectedCount + " / " + totalCollectibles + "\nTotal recolectado: " + totalSessionCollected;
        }

        Time.timeScale = 0f;
    }

    public void RestartFromFirstScene(string firstSceneName = "JardinOrquideas")
    {
        Time.timeScale = 1f;
        totalSessionCollected = 0;
        SceneManager.LoadScene(firstSceneName);
    }
}