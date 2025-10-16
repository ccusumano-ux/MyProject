using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public Vector2 velocity;
    public float lifetime = 5f;

    [Header("Physics & Spin")]
    public bool usePhysics = false;
    public bool spinOnSpeed = false;
    public float spinMultiplier = 360f;

    [Header("Impact Settings")]
    public bool stopOnGround = true;
    public bool stickToPlayer = true;

    public LayerMask groundLayer;                // assign Ground layer
    public LayerMask playerLayer;                // assign Player layer
    public string impactFloorBool = "Floor";     // Animator boolean for floor impact
    public string impactPlayerBool = "PlayerImpact"; // Animator boolean for player impact

    private Rigidbody2D rb;
    private Animator anim;

    // Player stick variables
    private bool hitGround = false;
    private bool hitPlayer = false;
    private Transform _stickTarget = null;
    private Vector3 _stickOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // optional; can be null
    }

    private void Start()
    {
        rb.linearVelocity = velocity;
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        // Maintain velocity if non-physics projectile and not impacted
        if (!usePhysics && !hitGround && !hitPlayer)
            rb.linearVelocity = velocity;

        // Spin based on speed
        if (spinOnSpeed)
        {
            float speed = rb.linearVelocity.magnitude;
            float spinDir = Mathf.Sign(rb.linearVelocity.x);
            transform.Rotate(Vector3.forward, -spinDir * speed * spinMultiplier * Time.fixedDeltaTime);
        }

        // Stick visually to player if impacted
        if (hitPlayer && _stickTarget != null)
            transform.position = _stickTarget.position + _stickOffset;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int layerMask = 1 << collision.gameObject.layer;

        // --- Ground Impact ---
        if (!hitGround && stopOnGround && (layerMask & groundLayer) != 0)
        {
            hitGround = true;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;

            if (anim != null && !string.IsNullOrEmpty(impactFloorBool))
                anim.SetBool(impactFloorBool, true);

            return;
        }

        // --- Player Impact ---
        if (!hitPlayer && stickToPlayer && (layerMask & playerLayer) != 0)
        {
            hitPlayer = true;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;

            _stickTarget = collision.transform;
            _stickOffset = transform.position - _stickTarget.position;

            if (anim != null && !string.IsNullOrEmpty(impactPlayerBool))
                anim.SetBool(impactPlayerBool, true);

            return;
        }
    }
}
