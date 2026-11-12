using UnityEngine;
using UnityEngine.UI;

public class MinimapPOI : MonoBehaviour
{
    [Header("Objetos en el Mundo 3D")]
    public Transform targetInWorld;       // Arrastra aquí la flor correspondiente (ej. Iteam_1_Orquidea)
    public Transform playerTransform;     // El Jugador

    [Header("Referencias de UI")]
    public RectTransform minimapRect;     // El objeto 'Mini_map' (círculo)
    public RectTransform iconRect;        // Este mismo icono en el Canvas

    [Header("Configuración del Radar")]
    public float maxWorldDistance = 30f;  // Distancia máxima que abarca el minimapa
    public float sensitivity = 1.0f;

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        if (iconRect == null) 
        {
            iconRect = GetComponent<RectTransform>();
        }
    }

    void LateUpdate()
    {
        // Si la flor ya fue recogida y destruida, ocultamos el icono del minimapa
        if (targetInWorld == null)
        {
            if (gameObject.activeSelf) gameObject.SetActive(false);
            return;
        }

        if (playerTransform == null || minimapRect == null || iconRect == null) return;

        // 1. Calculamos la distancia real en el mundo 3D entre el jugador y la flor
        Vector3 worldOffset = targetInWorld.position - playerTransform.position;

        // 2. Convertimos el espacio del mundo al radio del minimapa (ejes X y Z)
        float minimapRadius = (minimapRect.rect.width / 2f) - 10f;

        float normX = (worldOffset.x / maxWorldDistance) * sensitivity;
        float normY = (worldOffset.z / maxWorldDistance) * sensitivity; // Usamos Z para profundidad

        Vector2 uiPos = new Vector2(normX * minimapRadius, normY * minimapRadius);

        // 3. Si la flor está muy lejos, el icono se queda pegado en el borde del círculo
        if (uiPos.magnitude > minimapRadius)
        {
            uiPos = uiPos.normalized * minimapRadius;
        }

        // 4. Actualizamos la posición del icono dentro del minimapa
        iconRect.anchoredPosition = uiPos;
    }
}