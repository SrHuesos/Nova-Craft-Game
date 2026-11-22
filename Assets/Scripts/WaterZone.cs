using UnityEngine;

public class WaterZone : MonoBehaviour
{
    public string playerTag = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null) player.EnterWater();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null) player.ExitWater();
    }
}
