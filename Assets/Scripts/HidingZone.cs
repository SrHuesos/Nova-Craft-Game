using UnityEngine;

public class HidingZone : MonoBehaviour
{
    [Header("Configuración de Sigilo")]
    [Tooltip("Distancia máxima desde el centro del arbusto para considerarse oculto (ajustable).")]
    public float hidingThreshold = 1.2f;  

    private Transform zoneCenter;

    private void Awake()
    {
        zoneCenter = transform;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Calculamos tu lógica del 50% de distancia al centro del arbusto
            float distanceToCenter = Vector3.Distance(other.transform.position, zoneCenter.position);
            bool isInside = distanceToCenter <= hidingThreshold;

            // 2. Aplicamos tu lógica de movimiento / transparencia del jugador
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.SetHiding(isInside);
            }

            // 3. Integración con el sistema de Daniel (Hideable) para que el enemigo te ignore
            Hideable hideable = other.GetComponentInParent<Hideable>();
            if (hideable != null)
            {
                if (isInside && !hideable.IsHidden)
                {
                    hideable.Hide(); // Le avisa al enemigo que estás oculto por completo (cumpliste el 50%)
                    Debug.Log("Jugador oculto en el arbusto (50% alcanzado)");
                }
                else if (!isInside && hideable.IsHidden)
                {
                    hideable.Reveal(); // Te hace visible de nuevo si te sales del radio interno
                    Debug.Log("Jugador fuera del umbral de ocultamiento");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apagamos tu estado de ocultamiento
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.SetHiding(false);
            }

            // Hacemos visible al jugador para el enemigo de Daniel al salir por completo del arbusto
            Hideable hideable = other.GetComponentInParent<Hideable>();
            if (hideable != null)
            {
                hideable.Reveal();
                Debug.Log("Jugador salió del arbusto");
            }
        }
    }
}