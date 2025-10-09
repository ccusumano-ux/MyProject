using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public Vector2 velocity;
    public float lifetime = 5f;
    public bool usePhysics = false;       // If true, use Rigidbody2D physics (e.g., for pickaxes)
    public bool spinOnSpeed = false;      // Enable spinning based on velocity
    public float spinMultiplier = 360f;   // Rotation speed per unit of velocity

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (usePhysics)
        {
            rb.linearVelocity = velocity;
        }
        else
        {
            // Kinematic-style projectile (no physics)
            rb.gravityScale = 0;
            rb.linearVelocity = velocity;
        }

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (spinOnSpeed)
        {
            // Get current velocity magnitude
            float speed = rb.linearVelocity.magnitude;

            // Spin around Z-axis depending on direction and speed
            float spinDirection = Mathf.Sign(rb.linearVelocity.x);
            transform.Rotate(Vector3.forward, -spinDirection * speed * spinMultiplier * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Hit!");
            // Add your hit or death logic here
            Destroy(gameObject);
        }
    }
}
