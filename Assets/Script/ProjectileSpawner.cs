using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject fireballPrefab;
    public GameObject pickaxePrefab;
    public GameObject arrowPrefab;

    [Header("Spawn Settings")]
    public float spawnInterval = 1.5f;
    public float aimError = 1.5f;
    public float spawnDistance = 10f;

    [Header("Fireball Settings")]
    public float fireballMinSpeed = 4f;
    public float fireballMaxSpeed = 8f;

    [Header("Pickaxe Settings")]
    public float pickaxeMinSpeed = 5f;
    public float pickaxeMaxSpeed = 8f;

    [Header("Arrow Settings")]
    public float arrowMinSpeed = 8f;
    public float arrowMaxSpeed = 12f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnRandomProjectile();
        }
    }

    void SpawnRandomProjectile()
    {
        int type = Random.Range(0, 3);
        switch (type)
        {
            case 0:
                SpawnFireball();
                break;
            case 1:
                SpawnPickaxe();
                break;
            case 2:
                SpawnArrow();
                break;
        }
    }

    void SpawnFireball()
    {
        bool fromLeft = Random.value > 0.5f;
        Vector2 spawnPos = (Vector2)player.position + new Vector2(fromLeft ? -spawnDistance : spawnDistance, Random.Range(-2f, 2f));
        GameObject obj = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        Projectile p = obj.GetComponent<Projectile>();

        // Use inspector speed range
        float speed = Random.Range(fireballMinSpeed, fireballMaxSpeed);
        p.velocity = new Vector2(fromLeft ? speed : -speed, 0f);
    }

    void SpawnPickaxe()
    {
        bool fromLeft = Random.value > 0.5f;
        Vector2 spawnPos = (Vector2)player.position + new Vector2(fromLeft ? -spawnDistance : spawnDistance, 2f);
        GameObject obj = Instantiate(pickaxePrefab, spawnPos, Quaternion.identity);

        Projectile p = obj.GetComponent<Projectile>();

        float xForce = fromLeft ? Random.Range(pickaxeMinSpeed, pickaxeMaxSpeed) : Random.Range(-pickaxeMaxSpeed, -pickaxeMinSpeed);
        float yForce = Random.Range(pickaxeMinSpeed, pickaxeMaxSpeed);

        // Add small random multiplier for speed
        float speedMultiplier = Random.Range(0.8f, 1.2f);
        p.velocity = new Vector2(xForce * speedMultiplier, yForce * speedMultiplier);

        p.usePhysics = true;
        p.spinOnSpeed = true;
        p.spinMultiplier = 20f;
    }

    void SpawnArrow()
    {
        Vector2 spawnPos = new Vector2(player.position.x + Random.Range(-aimError, aimError), player.position.y + spawnDistance);
        GameObject obj = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

        Projectile p = obj.GetComponent<Projectile>();

        // Use inspector speed range
        float fallSpeed = Random.Range(arrowMinSpeed, arrowMaxSpeed);
        p.velocity = Vector2.down * fallSpeed;
    }


}


