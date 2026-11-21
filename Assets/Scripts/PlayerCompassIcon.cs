using UnityEngine;

public class PlayerCompassIcon : MonoBehaviour
{
    public Transform playerTransform; 
    public RectTransform iconRect;    

    [Header("Ajuste de Dirección (Offset)")]
    public float angleOffset = -90f; // Cambia a 0, 90 o -90 si la flecha apunta hacia otro lado

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }
        if (iconRect == null) iconRect = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (playerTransform == null || iconRect == null) return;

        // Toma la rotación del jugador y le aplica el offset para que apunte bien
        float playerYRot = playerTransform.eulerAngles.y;
        iconRect.localRotation = Quaternion.Euler(0, 0, -playerYRot + angleOffset);
    }
}