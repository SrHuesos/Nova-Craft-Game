using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 15f;

    [Header("Stealth / Sigilo (GDD)")]
    public bool isHiding = false;
    
    [Header("Stealth Visual Effect")]
    public Color normalColor = Color.white;
    public Color stealthTint = new Color(0.5f, 0.7f, 1f, 1f); // Tinte azulado/sigiloso

    private CharacterController characterController;
    private Animator animator;
    private SkinnedMeshRenderer[] skinRenderers;

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

        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;

        if (movement.magnitude >= 0.1f)
        {
            characterController.Move(movement * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (animator != null)
        {
            float speed = movement.magnitude;
            animator.SetFloat("Speed", speed);
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
        // Cambia el material a Opaque por seguridad para que nunca pierda texturas
        foreach (var renderer in skinRenderers)
        {
            foreach (var mat in renderer.materials)
            {
                // Aseguramos que conserve el modo Opaque
                if (mat.HasProperty("_Surface"))
                {
                    mat.SetFloat("_Surface", 0f); // 0 = Opaque en URP Lit
                }

                // Aplica un tinte visual de sigilo sin romper la textura ni el color base
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