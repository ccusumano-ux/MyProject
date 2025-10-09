using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float coyoteTime = 0.1f;          // Grace period after leaving ground
    public float jumpCutMultiplier = 0.5f;   // How much to reduce upward velocity when jump key released early

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private float lastGroundedTime;
    private bool jumpPressed;
    private bool isJumping;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // --- Get Movement Input ---
        moveInput.x = Input.GetAxisRaw("Horizontal");

        // --- Ground Check ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Track last time player was grounded (for coyote time)
        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            isJumping = false;
        }

        // --- Handle Jump Press ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpPressed = false;

            // Cut jump short if player released space early
            if (rb.linearVelocity.y > 0 && isJumping)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
                Debug.Log("Short Jump!");
            }
        }

        // --- Attempt Jump ---
        if (jumpPressed && (isGrounded || Time.time - lastGroundedTime <= coyoteTime))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            isJumping = true;
            Debug.Log("Jump!");
        }

        Debug.Log($"Move Input: {moveInput}, Grounded: {isGrounded}, Jumping: {isJumping}");
    }

    private void FixedUpdate()
    {
        // --- Apply Horizontal Movement ---
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

