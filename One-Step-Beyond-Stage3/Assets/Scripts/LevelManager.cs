using UnityEngine;

/// <summary>
/// Manages level collectibles count and unlocks the exit gate.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Game Progress")]
    public int totalCollectibles = 3;
    private int collectedCount = 0;

    [Header("Exit Gate Reference")]
    public GameObject exitGate;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCollectible()
    {
        collectedCount++;
        Debug.Log("Collectibles found: " + collectedCount + " / " + totalCollectibles);

        // Check if all items are collected to open the exit
        if (collectedCount >= totalCollectibles)
        {
            UnlockExit();
        }
    }

    private void UnlockExit()
    {
        Debug.Log("Exit gate unlocked! You can now progress to the next ecosystem.");
        
        if (exitGate != null)
        {
            // Disable the gate object or change its color/state
            exitGate.SetActive(false);
        }
    }
}