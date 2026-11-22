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

    [Header("Audio Sources")]
    public AudioSource footstepSource;
    public AudioSource bushSource;

    [Header("Footsteps on land")]
    public AudioClip[] footstepClips;
    [Range(0f, 1f)] public float footstepVolume = 0.6f;
    public float stepInterval = 0.4f;

    [Header("Footsteps in the bush")]
    public AudioClip[] bushFootstepClips;
    [Range(0f, 1f)] public float bushFootstepVolume = 0.5f;
    public float bushStepInterval = 0.6f;

    [Header("Footsteps in water")]
    public AudioClip[] waterFootstepClips;
    [Range(0f, 1f)] public float waterFootstepVolume = 0.7f;
    public float waterStepInterval = 0.5f;
    public AudioClip waterEnterClip;   // splash when entering (optional)
    public AudioClip waterExitClip;    // splash when leaving (optional)

    [Header("Enter / Exit the Bush")]
    public AudioClip bushEnterClip;
    public AudioClip bushExitClip;
    [Range(0f, 1f)] public float bushVolume = 0.8f;

    private float stepTimer;
    private int lastStepIndex = -1;
    private int waterContacts = 0;     // counts overlapping water zones

    public bool IsInWater => waterContacts > 0;

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

        if (footstepSource == null) footstepSource = CreateSource();
        if (bushSource == null) bushSource = CreateSource();

        footstepSource.playOnAwake = false;
        bushSource.playOnAwake = false;
    }

    private AudioSource CreateSource()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = 1f;
        source.maxDistance = 20f;
        return source;
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

            HandleFootsteps();
        }
        else
        {
            stepTimer = 0f;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", moveDirection.magnitude);
            animator.SetBool("IsHiding", isHiding);
        }
    }

    private void HandleFootsteps()
    {
        stepTimer -= Time.deltaTime;
        if (stepTimer > 0f) return;

        PlayFootstep();

        // If hiding, the slow stealth rhythm takes priority
        if (isHiding) stepTimer = bushStepInterval;
        else if (IsInWater) stepTimer = waterStepInterval;
        else stepTimer = stepInterval;
    }

    private void PlayFootstep()
    {
        footstepSource.pitch = Random.Range(0.9f, 1.1f);

        // Water layer
        if (IsInWater)
        {
            PlayRandomClip(waterFootstepClips, waterFootstepVolume);
        }

        // Bush layer
        if (isHiding)
        {
            PlayRandomClip(bushFootstepClips, bushFootstepVolume);
        }

        // Normal land: only if not in water and not hiding
        if (!IsInWater && !isHiding)
        {
            PlayRandomClip(footstepClips, footstepVolume);
        }
    }

    private void PlayRandomClip(AudioClip[] clips, float volume)
    {
        if (clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        if (clips.Length > 1 && index == lastStepIndex)
        {
            index = (index + 1) % clips.Length;
        }
        lastStepIndex = index;

        footstepSource.PlayOneShot(clips[index], volume);
    }

    private void PlayBushSound(bool entering)
    {
        AudioClip clip = entering ? bushEnterClip : bushExitClip;
        if (clip == null) return;

        bushSource.pitch = Random.Range(0.95f, 1.05f);
        bushSource.PlayOneShot(clip, bushVolume);
    }

    // Called by WaterZone
    public void EnterWater()
    {
        waterContacts++;
        if (waterContacts == 1)
        {
            lastStepIndex = -1;
            stepTimer = 0f;
            PlaySplash(waterEnterClip);
        }
    }

    public void ExitWater()
    {
        waterContacts = Mathf.Max(0, waterContacts - 1);
        if (waterContacts == 0)
        {
            lastStepIndex = -1;
            stepTimer = 0f;
            PlaySplash(waterExitClip);
        }
    }

    private void PlaySplash(AudioClip clip)
    {
        if (clip == null) return;

        bushSource.pitch = Random.Range(0.95f, 1.05f);
        bushSource.PlayOneShot(clip, waterFootstepVolume);
    }

    public void SetHiding(bool hidingState)
    {
        if (isHiding == hidingState) return;

        isHiding = hidingState;
        lastStepIndex = -1;
        UpdatePlayerStealthVisual(isHiding);
        PlayBushSound(isHiding);

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