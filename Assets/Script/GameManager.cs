using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI livesText;

    [Header("Score Settings")]
    public float basePPS = 10f;          // starting points per second
    public float incrementFactor = 0.1667f; // growth per second (~15 pps at 30s)

    private float currentScore = 0f;
    public float elapsedTime = 0f;
    private PlayerLife playerLife;

    private void Start()
    {
        playerLife = FindFirstObjectByType<PlayerLife>();
        UpdateUI();
    }

    private void Update()
    {
        if (playerLife != null && playerLife.currentLives > 0)
        {
            // increase time
            elapsedTime += Time.deltaTime;

            // calculate current PPS with linear growth
            float currentPPS = basePPS + elapsedTime * incrementFactor;

            // add score
            currentScore += currentPPS * Time.deltaTime;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        // --- Score & Time ---
        scoreText.text = $"Score: {Mathf.FloorToInt(currentScore)}";

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timeText.text = $"Time: {minutes:0}'{seconds:00}";

        // --- Lives ---
        if (playerLife != null)
        {
            livesText.text = $"Lives: {playerLife.currentLives}/{playerLife.maxLives}";
        }
    }
}
