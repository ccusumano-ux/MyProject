using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject fireballPrefab;
    public GameObject pickaxePrefab;
    public GameObject arrowPrefab;

    [Header("Base Spawn Settings")]
    public float baseSpawnInterval = 1.5f;
    public float aimError = 1.5f;
    public float spawnDistance = 10f;

    [Header("Fireball Settings")]
    public float fireballMinSpeed = 4f;
    public float fireballMaxSpeed = 8f;
    public float F_minY = 1f; 
    public float F_maxY = 4f;

    [Header("Pickaxe Settings")]
    public float pickaxeMinSpeed = 5f;
    public float pickaxeMaxSpeed = 8f;

    [Header("Arrow Settings")]
    public float arrowMinSpeed = 8f;
    public float arrowMaxSpeed = 12f;

    private float timer = 0f;
    private GameManager gameManager; // reference for elapsedTime

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (gameManager == null) return;

        // --- Difficulty multiplier based on elapsed time ---
        float elapsed = gameManager.elapsedTime; // public property
        float difficultyMultiplier = 1f + elapsed / 60f; // every 60s, multiplier +1
        float spawnInterval = baseSpawnInterval / difficultyMultiplier; // reduces interval

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnRandomProjectile(difficultyMultiplier);
        }
    }

    void SpawnRandomProjectile(float difficultyMultiplier)
    {
        int type = Random.Range(0, 3);
        switch (type)
        {
            case 0: SpawnFireball(difficultyMultiplier); break;
            case 1: SpawnPickaxe(difficultyMultiplier); break;
            case 2: SpawnArrow(difficultyMultiplier); break;
        }
    }

    void SpawnFireball(float difficultyMultiplier)
{
    bool fromLeft = Random.value > 0.5f;
    float spawnY = Mathf.Max(Random.Range(F_minY, F_maxY), F_minY);
    Vector2 spawnPos = new Vector2(player.position.x + (fromLeft ? -spawnDistance : spawnDistance), spawnY);

    GameObject obj = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
    Projectile p = obj.GetComponent<Projectile>();

    float speed = Random.Range(fireballMinSpeed, fireballMaxSpeed) * difficultyMultiplier;
    p.velocity = new Vector2(fromLeft ? speed : -speed, 0f);

    // --- Flip sprite using SpriteRenderer ---
    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
    if (sr != null)
        sr.flipX = !fromLeft; // flip if coming from right
}


    void SpawnPickaxe(float difficultyMultiplier)
    {
        bool fromLeft = Random.value > 0.5f;
        Vector2 spawnPos = (Vector2)player.position + new Vector2(fromLeft ? -spawnDistance : spawnDistance, 2f);

        GameObject obj = Instantiate(pickaxePrefab, spawnPos, Quaternion.identity);
        Projectile p = obj.GetComponent<Projectile>();

        float xForce = fromLeft ? Random.Range(pickaxeMinSpeed, pickaxeMaxSpeed) : Random.Range(-pickaxeMaxSpeed, -pickaxeMinSpeed);
        float yForce = Random.Range(pickaxeMinSpeed, pickaxeMaxSpeed);
        float speedMultiplier = Random.Range(0.8f, 1.2f) * difficultyMultiplier;

        p.velocity = new Vector2(xForce * speedMultiplier, yForce * speedMultiplier);
        p.usePhysics = true;
        p.spinOnSpeed = true;
        p.spinMultiplier = 20f;
    }

    void SpawnArrow(float difficultyMultiplier)
    {
        Vector2 spawnPos = new Vector2(player.position.x + Random.Range(-aimError, aimError), player.position.y + spawnDistance);
        GameObject obj = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

        Projectile p = obj.GetComponent<Projectile>();
        float fallSpeed = Random.Range(arrowMinSpeed, arrowMaxSpeed) * difficultyMultiplier;
        p.velocity = Vector2.down * fallSpeed;
    }
}
