using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float coyoteTime = 0.1f;
    public float jumpCutMultiplier = 0.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Wall Check (Tag Based)")]
    public float wallCheckDistance = 0.3f;
    public string wallTag = "Wall";

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
        moveInput.x = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            isJumping = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            jumpPressed = true;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpPressed = false;
            if (rb.linearVelocity.y > 0 && isJumping)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        if (jumpPressed && (isGrounded || Time.time - lastGroundedTime <= coyoteTime))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        float moveDir = moveInput.x;
        bool touchingWall = false;

        if (moveDir != 0)
        {
            RaycastHit2D wallHit = Physics2D.Raycast(transform.position, new Vector2(moveDir, 0), wallCheckDistance);
            if (wallHit.collider != null && wallHit.collider.CompareTag(wallTag))
                touchingWall = true;
        }

        rb.linearVelocity = new Vector2(touchingWall ? 0f : moveDir * moveSpeed, rb.linearVelocity.y);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.blue;
        if (Application.isPlaying)
        {
            Vector2 dir = new Vector2(Mathf.Sign(moveInput.x), 0);
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + dir * wallCheckDistance);
        }
    }

}
//Version 0001