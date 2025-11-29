using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float rotationSpeed = 10f;
    public Transform cameraTransform;

    [Header("Jump")]
    public float jumpForce = 5f;
    public float jumpCooldown = 0.4f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Stamina")]
    public float maxStamina = 5f;
    public float staminaRecoveryRate = 1f;
    public float staminaDrainRate = 1.5f;

    [Header("Audio")]
    public AudioSource footstepAudioSource;
    public AudioSource sfxAudioSource;
    public AudioClip walkClip;
    public AudioClip runClip;
    public AudioClip jumpClip;
    public AudioClip deathClip;

    private Rigidbody rb;
    private Animator animator;

    private float currentStamina;
    private bool isRunning;
    private bool isGrounded;
    private bool canJump = true;

    private Vector3 inputDirection;
    private bool runCooldown = false;
    private float runCooldownTimer = 1f;
    private float currentCooldownTime = 0f;
    private Vector3 spawnPoint;
    private PlatformTrigger currentTrigger;
    private DialogueController currentDialogue;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentStamina = maxStamina;
        spawnPoint = transform.position;
        animator = GetComponent<Animator>();

        if (footstepAudioSource != null)
            footstepAudioSource.loop = true;
    }

    private void Update()
    {
        HandleInput();
        CheckGround();

        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();

        if (currentTrigger != null && Input.GetKeyDown(KeyCode.E))
            currentTrigger.ActivatePlatform();

        if (currentDialogue != null && Input.GetKeyDown(KeyCode.E))
            currentDialogue.StartDialogue();
    }

    private void FixedUpdate()
    {
        Move();
        HandleStamina();
        BetterFall();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        isRunning = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0f && !runCooldown;
    }

    private void Move()
    {
        bool isWalkingNow = inputDirection.magnitude > 0.1f;

        if (!isWalkingNow)
        {
            animator.SetFloat("Speed", 0f);
            StopFootsteps();
            return;
        }

        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 moveDir = camForward * inputDirection.z + camRight * inputDirection.x;

        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsRunning", isRunning);

        PlayFootsteps();
    }

    private void PlayFootsteps()
    {
        if (!isGrounded || footstepAudioSource == null) return;

        AudioClip selectedClip = isRunning ? runClip : walkClip;

        if (!footstepAudioSource.isPlaying || footstepAudioSource.clip != selectedClip)
        {
            footstepAudioSource.clip = selectedClip;
            footstepAudioSource.Play();
        }
    }

    private void StopFootsteps()
    {
        if (footstepAudioSource != null && footstepAudioSource.isPlaying)
            footstepAudioSource.Stop();
    }

    private void TryJump()
    {
        if (!isGrounded || !canJump) return;

        canJump = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        animator.SetBool("IsJumping", true);
        StopFootsteps();

        if (jumpClip != null)
            sfxAudioSource.PlayOneShot(jumpClip);

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    void ResetJump() => canJump = true;

    private void BetterFall()
    {
        if (!isGrounded)
            rb.AddForce(Vector3.down * 10f, ForceMode.Acceleration);
    }

    private void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded && !wasGrounded)
            animator.SetBool("IsJumping", false);

        if (!isGrounded) StopFootsteps();
    }

    private void HandleStamina()
    {
        if (isRunning && !runCooldown)
            currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
        else
            currentStamina += staminaRecoveryRate * Time.fixedDeltaTime;

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (currentStamina <= 0f && !runCooldown)
        {
            runCooldown = true;
            currentCooldownTime = runCooldownTimer;
            animator.SetBool("IsRunning", false);
        }

        if (runCooldown)
        {
            currentCooldownTime -= Time.fixedDeltaTime;
            if (currentCooldownTime <= 0f)
                runCooldown = false;
        }
    }

    private void Respawn()
    {
        rb.velocity = Vector3.zero;
        transform.position = spawnPoint;
        StopFootsteps();

        if (deathClip != null)
            sfxAudioSource.PlayOneShot(deathClip);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlatformTrigger trigger))
            currentTrigger = trigger;

        if (other.CompareTag("CheckPoint"))
            spawnPoint = other.transform.position;

        if (other.CompareTag("Kill"))
            Respawn();

        if (other.CompareTag("DialogStart"))
        {
            if (other.TryGetComponent(out DialogueController dialogue))
                currentDialogue = dialogue;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlatformTrigger trigger) && currentTrigger == trigger)
            currentTrigger = null;

        if (other.CompareTag("DialogStart"))
        {
            if (other.TryGetComponent(out DialogueController dialogue) && currentDialogue == dialogue)
                currentDialogue = null;
        }
    }

    public float GetStamina() => currentStamina / maxStamina;
}
