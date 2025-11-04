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
    public float basePPS = 10f;
    public float incrementFactor = 0.1667f;

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
        if (playerLife.currentLives <= 0) return;

        elapsedTime += Time.deltaTime;
        float currentPPS = basePPS + elapsedTime * incrementFactor;
        currentScore += currentPPS * Time.deltaTime;

        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = $"Score: {Mathf.FloorToInt(currentScore)}";

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timeText.text = $"Time: {minutes:0}'{seconds:00}";

        if (playerLife != null)
            livesText.text = $"Lives: {playerLife.currentLives}/{playerLife.maxLives}";
    }

    private void HandlePlayerDeath()
    {
        if (gameOver) return;
        gameOver = true;

        int finalScore = Mathf.FloorToInt(currentScore);

        if (highScoreManager != null)
        {
            if (playerLife.debugDeathTriggered)
            {
                highScoreManager.ForceDebugScore(finalScore);
            }
            else
            {
                highScoreManager.ShowPanel(finalScore);
            }
        }

        UpdateUI();
        Time.timeScale = 0f;
    }

}
//Version 0001