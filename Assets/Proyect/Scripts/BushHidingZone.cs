
using UnityEngine;

public class BushHidingZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Hideable hideable = other.GetComponentInParent<Hideable>();

        if (hideable != null)
        {
            hideable.Hide();
            Debug.Log("Jugador escondido en los arbustos");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Hideable hideable = other.GetComponentInParent<Hideable>();

        if (hideable != null)
        {
            hideable.Reveal();
            Debug.Log("Jugador salió de los arbustos");
        }
    }
}
