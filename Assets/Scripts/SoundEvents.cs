using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FindEvent : MonoBehaviour
{
    [Tooltip("If enabled, the sound only plays the first time the player enters")]
    public bool soloUnaVez = true;

    [Tooltip("If enabled, the sound stops playing when the player exits the area")]
    public bool detenerAlSalir = false;

    private AudioSource audioSource;
    private bool yaSono;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (soloUnaVez && yaSono) return;

        audioSource.Play();
        yaSono = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!detenerAlSalir) return;
        if (!other.CompareTag("Player")) return;

        audioSource.Stop();
    }
}