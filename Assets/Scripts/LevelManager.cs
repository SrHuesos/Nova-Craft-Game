using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Game Progress")]
    public int totalCollectibles = 3;
    private int collectedCount = 0;

    [Header("UI References")]
    public TMP_Text titleMissionText; // Arrastra Text_title_mission aquí
    public TMP_Text descriptionMissionText; // Arrastra Text_mission aquí
    public GameObject exitGate;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (titleMissionText != null) titleMissionText.text = "MISIÓN ACTUAL";
        UpdateMissionUI();
    }

    public void AddCollectible()
    {
        collectedCount++;
        UpdateMissionUI();

        if (collectedCount >= totalCollectibles)
        {
            UnlockExit();
        }
    }

    private void UpdateMissionUI()
    {
        if (descriptionMissionText != null)
        {
            if (collectedCount < totalCollectibles)
            {
                descriptionMissionText.text = "Busca las flores endémicas (" + collectedCount + "/" + totalCollectibles + ")";
            }
            else
            {
                descriptionMissionText.text = "¡Especies completas! Ve a la puerta de salida.";
            }
        }
    }

    private void UnlockExit()
    {
        if (exitGate != null) exitGate.SetActive(false);
    }
}