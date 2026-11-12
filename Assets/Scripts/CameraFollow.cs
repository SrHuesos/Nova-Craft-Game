using UnityEngine;

/// <summary>
/// Makes the main camera smoothly follow the player's position in the ecosystem.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Drag the Player here
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 8f, -8f); // Isometric offset

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position based on player and offset
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move the camera towards the target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}