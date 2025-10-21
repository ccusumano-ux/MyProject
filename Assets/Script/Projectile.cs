using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public Vector2 velocity;
    public float lifetime = 5f;
    public int damage = 1;

    [Header("Physics & Spin")]
    public bool usePhysics = false;
    public bool spinOnSpeed = false;
    public float spinMultiplier = 360f;

    [Header("Impact Settings")]
    public bool stopOnGround = true;
    public bool stickToPlayer = true;

    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public string impactFloorBool = "Floor";
    public string impactPlayerBool = "PlayerImpact";

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;

    private bool hitGround = false;
    private bool hitPlayer = false;
    private Transform _stickTarget = null;
    private Vector3 _stickOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        rb.linearVelocity = velocity;
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (!usePhysics && !hitGround && !hitPlayer)
            rb.linearVelocity = velocity;

        if (spinOnSpeed)
        {
            float speed = rb.linearVelocity.magnitude;
            float spinDir = Mathf.Sign(rb.linearVelocity.x);
            transform.Rotate(Vector3.forward, -spinDir * speed * spinMultiplier * Time.fixedDeltaTime);
        }

        if (hitPlayer && _stickTarget != null)
            transform.position = _stickTarget.position + _stickOffset;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int layerMask = 1 << collision.gameObject.layer;

        // --- Ground Impact ---
        if (!hitGround && stopOnGround && (layerMask & groundLayer) != 0)
        {
            HandleGroundImpact();
            return;
        }

        // --- Player Impact ---
        if (!hitPlayer && stickToPlayer && (layerMask & playerLayer) != 0)
        {
            HandlePlayerImpact(collision);
            return;
        }
    }

    private void HandleGroundImpact()
    {
        hitGround = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (col != null)
            col.enabled = false; // ✅ stop further collisions

        if (anim != null && !string.IsNullOrEmpty(impactFloorBool))
            anim.SetBool(impactFloorBool, true);
    }

    private void HandlePlayerImpact(Collision2D collision)
    {
        hitPlayer = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (col != null)
            col.enabled = false; // ✅ disable collisions so it doesn’t block player

        _stickTarget = collision.transform;
        _stickOffset = transform.position - _stickTarget.position;

        // 🎯 Apply Damage
        PlayerLife playerLife = collision.gameObject.GetComponent<PlayerLife>();
        if (playerLife != null)
            playerLife.TakeDamage(damage);

        if (anim != null && !string.IsNullOrEmpty(impactPlayerBool))
            anim.SetBool(impactPlayerBool, true);
    }
}
