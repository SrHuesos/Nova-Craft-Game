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

    void Start()
    {
        currentHealth = maxHealth;

        // Asegura que el panel de Game Over comience apagado al iniciar el juego
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateHeartsUI();
    }

    void Update()
    {
        // TECLA TEMPORAL DE PRUEBA: Presiona 'H' para recibir 1 de daño y probar los corazones
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            TriggerGameOver();
        }
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (i < currentHealth)
            {
                heartIcons[i].SetActive(true);
            }
            else
            {
                heartIcons[i].SetActive(false); // Apaga de derecha a izquierda
            }
        }
    }

    void TriggerGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f; // Pausa el juego
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Reanuda el tiempo antes de recargar
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}