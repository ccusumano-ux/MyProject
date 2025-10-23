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
    public bool destroyOnContact = false; // 🔥 new toggle

    public LayerMask groundLayer;
    public string playerTag = "Player";
    public string impactFloorBool = "Floor";
    public string impactPlayerBool = "PlayerImpact";

    // internal refs
    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;

    // state
    private bool hitGround = false;
    private bool hitPlayer = false;
    private Transform _stickTarget = null;
    private Vector3 _stickOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>(); // optional
    }

    private void Start()
    {
        rb.linearVelocity = velocity;
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        
        // keep moving if it's a kinematic projectile
        if (!usePhysics && !hitGround && !hitPlayer)
            rb.linearVelocity = velocity;

        // spin visual
        if (spinOnSpeed)
        {
            float speed = rb.linearVelocity.magnitude;
            float spinDir = Mathf.Sign(rb.linearVelocity.x);
            transform.Rotate(Vector3.forward, -spinDir * speed * spinMultiplier * Time.fixedDeltaTime);
        }

        // visual stick to player (no physics)
        if (hitPlayer && _stickTarget != null)
            transform.position = _stickTarget.position + _stickOffset;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleContact(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleContact(other.gameObject);
    }

    private void HandleContact(GameObject other)
    {
        if (other == null) return;

        // 1) Ground impact
        int otherLayerMask = 1 << other.layer;
        if (!hitGround && stopOnGround && (otherLayerMask & groundLayer) != 0)
        {
            HandleGroundImpact();
            return;
        }

        // 2) Player contact
        if (!hitPlayer && other.CompareTag(playerTag))
        {
            HandlePlayerContact(other);
            return;
        }
    }

    private void HandleGroundImpact()
    {
        hitGround = true;

        if (destroyOnContact)
        {
            DestroyProjectile();
            return;
        }

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        if (col != null) col.enabled = false;

        if (anim != null && !string.IsNullOrEmpty(impactFloorBool))
            anim.SetBool(impactFloorBool, true);
    }

    private void HandlePlayerContact(GameObject playerObj)
    {

        if (hitGround) return;
        PlayerLife playerLife = playerObj.GetComponentInParent<PlayerLife>();
        if (playerLife != null)
            playerLife.TakeDamage(damage);

        if (destroyOnContact)
        {
            DestroyProjectile();
            return;
        }

        hitPlayer = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        if (col != null) col.enabled = false;

        if (stickToPlayer)
        {
            _stickTarget = playerObj.transform;
            _stickOffset = transform.position - _stickTarget.position;
        }

        if (anim != null && !string.IsNullOrEmpty(impactPlayerBool))
            anim.SetBool(impactPlayerBool, true);
    }

    private void DestroyProjectile()
    {
        if (anim != null)
        {
            // play impact animation one frame before destroy
            if (!string.IsNullOrEmpty(impactPlayerBool))
                anim.SetBool(impactPlayerBool, true);
            if (!string.IsNullOrEmpty(impactFloorBool))
                anim.SetBool(impactFloorBool, true);
        }

        Destroy(gameObject);
    }
}
