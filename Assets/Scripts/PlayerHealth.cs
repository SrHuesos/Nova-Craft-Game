using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Salud")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Referencias de UI")]
    public GameObject gameOverPanel;
    public GameObject[] heartIcons;  // [0] = izq, [1] = centro, [2] = der

    [Header("Audio")]
    public AudioSource damageSource;
    public AudioClip[] damageClips;
    [Range(0f, 1f)] public float damageVolume = 0.8f;

    public AudioClip gameOverClip;
    [Range(0f, 1f)] public float gameOverVolume = 1f;

    private int lastDamageIndex = -1;

    void Start()
    {
        currentHealth = maxHealth;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateHeartsUI();
    }

    void Update()
    {
        // TECLA TEMPORAL DE PRUEBA: Presiona 'H' para recibir 1 de daño
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Si este golpe te mata, suena el de game over en vez del de daño
        if (currentHealth > 0)
        {
            PlayDamageSound();
        }

        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            TriggerGameOver();
        }
    }

    void PlayDamageSound()
    {
        if (damageSource == null || damageClips == null || damageClips.Length == 0) return;

        int index = Random.Range(0, damageClips.Length);

        // Si hay más de un clip y salió el mismo que la vez anterior, pasa al siguiente
        if (damageClips.Length > 1 && index == lastDamageIndex)
        {
            index = (index + 1) % damageClips.Length;
        }
        lastDamageIndex = index;

        damageSource.pitch = Random.Range(0.95f, 1.05f);
        damageSource.PlayOneShot(damageClips[index], damageVolume);
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < currentHealth);
        }
    }

    void TriggerGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (damageSource != null && gameOverClip != null)
        {
            damageSource.pitch = 1f;
            damageSource.PlayOneShot(gameOverClip, gameOverVolume);
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}