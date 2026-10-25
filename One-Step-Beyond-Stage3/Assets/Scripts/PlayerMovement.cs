using UnityEngine;

/// <summary>
/// Handles basic player movement on the ground.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
        }
    }

    private void Update()
    {
        // Read horizontal and vertical keyboard input.
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Create movement relative to the world axes.
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;

        // Move the character while keeping movement speed consistent.
        characterController.Move(movement * moveSpeed * Time.deltaTime);
    }
}