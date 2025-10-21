using UnityEngine;
using System.Collections;

public class SpikeManager : MonoBehaviour
{
    [Header("Spike Settings")]
    public GameObject spikePrefab;
    public float activationDelay = 10f;  // When spikes appear
    public float spikeDuration = 5f;     // How long spikes stay
    public float spikeSpacing = 1f;
    public float floorY = -3f;
    public float spawnWidth = 20f;

    [Header("Temporary Platforms")]
    public GameObject platformPrefab;
    public int platformCount = 3;
    public float platformY = 1f;          // Height of platforms
    public float platformDuration = 2f;   // How long platforms stay

    private GameObject[] spawnedSpikes;
    private GameObject[] spawnedPlatforms;

    private void Start()
    {
        StartCoroutine(SpikeEventSequence());
    }

    private IEnumerator SpikeEventSequence()
    {
        // --- 1 second before spikes: spawn platforms ---
        yield return new WaitForSeconds(activationDelay - 1f);
        SpawnPlatforms();

        // --- Wait 1 second to spawn spikes ---
        yield return new WaitForSeconds(1f);
        SpawnSpikes();

        // --- Wait for spikes duration ---
        yield return new WaitForSeconds(spikeDuration);
        DespawnSpikes();

        // --- Wait platform duration ---
        yield return new WaitForSeconds(platformDuration);
        DespawnPlatforms();
    }

    #region Spike Methods
    private void SpawnSpikes()
    {
        int spikeCount = Mathf.RoundToInt(spawnWidth / spikeSpacing);
        spawnedSpikes = new GameObject[spikeCount];

        float startX = -spawnWidth / 2f;

        for (int i = 0; i < spikeCount; i++)
        {
            Vector3 spawnPos = new Vector3(startX + i * spikeSpacing, floorY, 0f);
            spawnedSpikes[i] = Instantiate(spikePrefab, spawnPos, Quaternion.identity);
        }

        Debug.Log("⚠️ Spikes Activated!");
    }

    private void DespawnSpikes()
    {
        if (spawnedSpikes == null) return;

        foreach (GameObject s in spawnedSpikes)
        {
            if (s != null) Destroy(s);
        }

        Debug.Log("🟢 Spikes Deactivated!");
    }
    #endregion

    #region Platform Methods
    private void SpawnPlatforms()
    {
        spawnedPlatforms = new GameObject[platformCount];

        float spacing = spawnWidth / (platformCount + 1);

        for (int i = 0; i < platformCount; i++)
        {
            Vector3 spawnPos = new Vector3(-spawnWidth / 2f + spacing * (i + 1), platformY, 0f);
            spawnedPlatforms[i] = Instantiate(platformPrefab, spawnPos, Quaternion.identity);
        }

        Debug.Log("🟡 Temporary Platforms Spawned!");
    }

    private void DespawnPlatforms()
    {
        if (spawnedPlatforms == null) return;

        foreach (GameObject p in spawnedPlatforms)
        {
            if (p != null) Destroy(p);
        }

        Debug.Log("🔵 Temporary Platforms Removed!");
    }
    #endregion
}
