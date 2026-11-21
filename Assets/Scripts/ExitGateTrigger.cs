using UnityEngine;

public class ExitGateTrigger : MonoBehaviour
{
    [Header("Audio Optional")]
    public AudioSource audioSource;
    public AudioClip enterGateClip;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (audioSource != null && enterGateClip != null)
            {
                audioSource.PlayOneShot(enterGateClip);
            }

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.TriggerExitGate();
            }
        }
    }
}