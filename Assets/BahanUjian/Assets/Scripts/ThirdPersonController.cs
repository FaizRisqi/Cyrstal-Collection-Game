using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -30f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpSound;

    // Components
    private CharacterController controller;
    private Transform cameraTransform;
    private Animator animator;
    private AudioSource audioSource;
    
    // Movement variables
    private Vector3 velocity;
    private bool isGrounded;
    private bool canJump = true; // FIXED: Add jump control
    private float coyoteTime = 0.1f; // Small buffer for ground check
    private float coyoteTimeCounter;
    private float turnSmoothVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // FIXED: Proper ground check with coyote time
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Coyote time - small buffer after leaving ground
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            // canJump = true; // Dipindah ke bawah biar lebih aman
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Reset downward velocity when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small force to keep grounded
            canJump = true; // RESET LOMPAT DISINI (SANGAT PENTING UNTUK DOUBLE JUMP)
            
            if (animator != null)
            {
                animator.SetBool("IsJumping", false);
            }
        }

        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Movement
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            if (animator != null)
            {
                animator.SetFloat("Speed", currentSpeed);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
            }
        }

        // FIXED: Jump - Hanya sekali saat di tanah
        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            canJump = false; // KUNCI DISINI AGAK TIDAK DOUBLE JUMP
            coyoteTimeCounter = 0f; 
            
            if (animator != null)
            {
                animator.SetBool("IsJumping", true);
            }
            
            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
            
            Debug.Log("Jump executed");
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        
        // FIXED: Use unscaledDeltaTime to prevent timer bug
        controller.Move(velocity * Time.deltaTime);

        // Unlock cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}