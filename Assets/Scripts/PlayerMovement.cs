using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 15f;
    public float gravity = -20f;

    [Header("Stealth / Sigilo (GDD)")]
    public bool isHiding = false;

    [Header("Stealth Visual Effect")]
    public Color normalColor = Color.white;
    public Color stealthTint = new Color(0.5f, 0.7f, 1f, 1f);

    private CharacterController characterController;
    private Animator animator;
    private SkinnedMeshRenderer[] skinRenderers;
    private float verticalVelocity = 0f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
        }

        animator = GetComponentInChildren<Animator>();
        skinRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 finalVelocity = moveDirection * moveSpeed;
        finalVelocity.y = verticalVelocity;

        characterController.Move(finalVelocity * Time.deltaTime);

        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", moveDirection.magnitude);
            animator.SetBool("IsHiding", isHiding);
        }
    }

    public void SetHiding(bool hidingState)
    {
        if (isHiding == hidingState) return;

        isHiding = hidingState;
        UpdatePlayerStealthVisual(isHiding);

        if (animator != null)
        {
            animator.SetBool("IsHiding", isHiding);
        }

        if (isHiding)
        {
            Debug.Log("Emma ingresó al arbusto. Modo sigilo ACTIVADO.");
        }
        else
        {
            Debug.Log("Emma salió del arbusto. Modo sigilo DESACTIVADO.");
        }
    }

    private void UpdatePlayerStealthVisual(bool hiding)
    {
        foreach (var renderer in skinRenderers)
        {
            foreach (var mat in renderer.materials)
            {
                if (mat.HasProperty("_Surface"))
                {
                    mat.SetFloat("_Surface", 0f);
                }

                if (mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", hiding ? stealthTint : normalColor);
                }
                else if (mat.HasProperty("_Color"))
                {
                    mat.color = hiding ? stealthTint : normalColor;
                }
            }
        }
    }
}