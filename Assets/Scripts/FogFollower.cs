using UnityEngine;

/// <summary>
/// Makes the fog of war plane smoothly follow the player's XZ position.
/// </summary>
public class FogFollower : MonoBehaviour
{
    [Header("Target")]
    public Transform playerTransform;
    public float heightOffset = 3f;

    void LateUpdate()
    {
        if (playerTransform == null) return;

        // Keep the fog centered above the player, ignoring Y axis changes
        Vector3 targetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y + heightOffset, playerTransform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
    }
}