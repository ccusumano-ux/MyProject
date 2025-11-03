using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    private bool gameOver = false;

    private PlayerLife playerLife;
    private HighScoreManager highScoreManager;

    private void Start()
    {
        playerLife = FindFirstObjectByType<PlayerLife>();
        highScoreManager = FindFirstObjectByType<HighScoreManager>();

        if (playerLife != null)
            playerLife.onDeath.AddListener(HandlePlayerDeath);

        UpdateUI();
    }

    private void Update()
    {
        if (playerLife == null || gameOver) return;
        if (playerLife.currentLives <= 0) return; // avoid score ticking after death

        // --- Increase time and score ---
        elapsedTime += Time.deltaTime;
        float currentPPS = basePPS + elapsedTime * incrementFactor;
        currentScore += currentPPS * Time.deltaTime;

        UpdateUI();
    }

    private void UpdateUI()
    {
        // --- Score ---
        scoreText.text = $"Score: {Mathf.FloorToInt(currentScore)}";

        // --- Time ---
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timeText.text = $"Time: {minutes:0}'{seconds:00}";

        // --- Lives ---
        if (playerLife != null)
        {
            livesText.text = $"Lives: {playerLife.currentLives}/{playerLife.maxLives}";
        }
    }

    private void HandlePlayerDeath()
    {
        if (gameOver) return;

        gameOver = true;
        int finalScore = Mathf.FloorToInt(currentScore);

        if (highScoreManager != null)
            highScoreManager.ShowPanel(finalScore);

        UpdateUI(); // ensures lives show 0/3
        Time.timeScale = 0f;
        highScoreManager.ShowPanel(Mathf.FloorToInt(currentScore));
    }
}
