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

    [Header("Spawn")]
    public GameObject spawnPrefab;
    public Vector3 spawnOffset = new Vector3(0, 1f, 0);

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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentStamina = maxStamina;
        spawnPoint = transform.position;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleInput();
        CheckGround();

        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();

        if (Input.GetKeyDown(KeyCode.Q))
            SpawnObject();

        if (currentTrigger != null && Input.GetKeyDown(KeyCode.E))
            currentTrigger.ActivatePlatform();
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
        if (inputDirection.magnitude < 0.1f)
        {
            animator.SetFloat("Speed", 0f);
            isRunning = false;
            animator.SetBool("IsRunning", false);
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
    }

    private void TryJump()
    {
        if (!isGrounded || !canJump) return;

        canJump = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        animator.SetBool("IsJumping", true);

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

    private void SpawnObject()
    {
        if (spawnPrefab == null) return;
        Instantiate(spawnPrefab, transform.position + spawnOffset, Quaternion.identity);
    }

    private void Respawn()
    {
        rb.velocity = Vector3.zero;
        transform.position = spawnPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlatformTrigger trigger))
            currentTrigger = trigger;

        if (other.CompareTag("CheckPoint"))
            spawnPoint = other.transform.position;

        if (other.CompareTag("Kill"))
            Respawn();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlatformTrigger trigger) && currentTrigger == trigger)
            currentTrigger = null;
    }

    public float GetStamina() => currentStamina / maxStamina;
}
