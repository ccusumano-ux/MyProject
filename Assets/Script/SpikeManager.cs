using UnityEngine;

public class SpikeManager : MonoBehaviour
{
    [Header("References")]
    public GameObject spikePrefab;
    public GameObject platformPrefab;

    [Header("Floor Settings")]
    public float spikeY = 0f;         // Y position of spikes (floor)
    public int spikeCount = 20;       // number of spikes to cover the floor
    public float spikeSpacing = 1f;   // distance between each spike

    [Header("Platform Settings")]
    public Transform[] platformSpawnPoints;
    public float platformAdvanceTime = 1f;
    public float platformDisappearDelay = 2f;

    [Header("Spawn Timing")]
    public float minSpawnInterval = 0f;
    public float maxSpawnInterval = 30f;
    public float spikeActiveTime = 5f;

    private float timer = 0f;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        timer = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void Update()
    {
        if (gameManager == null) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnSpikesAndPlatforms();

            // reset timer with difficulty scaling
            float difficultyMultiplier = 1f + gameManager.elapsedTime / 60f;
            timer = Random.Range(minSpawnInterval, maxSpawnInterval) / difficultyMultiplier;
        }
    }

    private void SpawnSpikesAndPlatforms()
    {
        // --- Spawn platforms ---
        if (platformSpawnPoints != null && platformSpawnPoints.Length > 0)
        {
            int numPlatforms = Mathf.Max(1, 5 - Mathf.FloorToInt(gameManager.elapsedTime / 30f));
            for (int i = 0; i < numPlatforms; i++)
            {
                Transform spawnPoint = platformSpawnPoints[Random.Range(0, platformSpawnPoints.Length)];
                GameObject platform = Instantiate(platformPrefab, spawnPoint.position, Quaternion.identity);
                Destroy(platform, spikeActiveTime + platformDisappearDelay);
            }
        }

        // --- Spawn spikes across the floor ---
        StartCoroutine(SpawnSpikesCoroutine());
    }

    private System.Collections.IEnumerator SpawnSpikesCoroutine()
    {
        yield return new WaitForSeconds(platformAdvanceTime);

        float startX = -((spikeCount - 1) * spikeSpacing) / 2f; // center spikes around 0
        for (int i = 0; i < spikeCount; i++)
        {
            Vector3 pos = new Vector3(startX + i * spikeSpacing, spikeY, 0f);
            GameObject spike = Instantiate(spikePrefab, pos, Quaternion.identity);
            Destroy(spike, spikeActiveTime);
        }
    }
}
