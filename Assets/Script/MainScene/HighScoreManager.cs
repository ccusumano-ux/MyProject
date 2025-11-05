using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class HighScoreManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject deathCanvas;
    public GameObject darkPanel;
    public GameObject boxPanel;

    public TextMeshProUGUI textYouDied;
    public TextMeshProUGUI textHighScore;
    public TextMeshProUGUI textPoints;
    public TMP_InputField nameInputField;
    public GameObject nameEntryPanel;
    public GameObject buttonRetry;
    public GameObject buttonMenu;

    [Header("Online Score API")]
    public GoogleSheetsAPI googleSheetAPI;

    private GoogleSheetsAPI.ScoreData[] onlineScores;
    private int pendingScore = 0;
    private bool isNewHighScore = false;

    private void Start()
    {
        if (googleSheetAPI == null)
        {
            Debug.LogError("HighScoreManager: GoogleSheetAPI not assigned!");
            return;
        }

        googleSheetAPI.onGetLeaderboard += OnLeaderboardReceived;

        // ✅ Use cached leaderboard if available
        if (MainMenuLeaderboard.cachedLeaderboard != null && MainMenuLeaderboard.cachedLeaderboard.Count > 0)
        {
            onlineScores = MainMenuLeaderboard.cachedLeaderboard
                            .OrderByDescending(s => s.score)
                            .Take(5)
                            .ToArray();
            Debug.Log("HighScoreManager: Using cached leaderboard from MainMenuLeaderboard.");
        }
        else
        {
            googleSheetAPI.GetLeaderboard();
        }

        if (nameInputField != null)
            nameInputField.onSubmit.AddListener(_ => ConfirmName());
    }

    private void OnLeaderboardReceived(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        try
        {
            onlineScores = JsonHelper.FromJson<GoogleSheetsAPI.ScoreData>(json)
                            .Where(s => s.score > 0)
                            .OrderByDescending(s => s.score)
                            .Take(5)
                            .ToArray();

            // ✅ Update global cache
            MainMenuLeaderboard.cachedLeaderboard = onlineScores.ToList();

            DisplayScores();
            Debug.Log($"HighScoreManager: Leaderboard loaded with {onlineScores.Length} entries.");
        }
        catch (Exception e)
        {
            Debug.LogError("HighScoreManager: Failed to parse leaderboard JSON: " + e.Message);
        }
    }

    public void ShowPanel(int finalScore)
    {
        deathCanvas.SetActive(true);
        darkPanel.SetActive(true);
        boxPanel.SetActive(true);
        textYouDied.text = "YOU DIED";
        textHighScore.text = "HIGHSCORES";

        pendingScore = finalScore;

        if (onlineScores == null || onlineScores.Length == 0)
        {
            Debug.Log("HighScoreManager: Leaderboard not ready — treating as not new high score.");
            isNewHighScore = false;
        }
        else
        {
            int lowestScore = onlineScores.Min(s => s.score);
            isNewHighScore = finalScore > lowestScore;
        }

        if (isNewHighScore)
        {
            nameEntryPanel.SetActive(true);
            textPoints.text = "NEW HIGHSCORE!\nEnter your name:";
        }
        else
        {
            nameEntryPanel.SetActive(false);
            DisplayScores();
        }
    }

    public void ConfirmName()
    {
        string playerName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(playerName)) playerName = "AAA";
        if (playerName.Length > 6) playerName = playerName.Substring(0, 6);

        googleSheetAPI.onPostResult += OnScoreSubmitted;
        googleSheetAPI.PostScore(playerName, pendingScore);

        nameEntryPanel.SetActive(false);
    }

    private void OnScoreSubmitted(bool success)
    {
        googleSheetAPI.GetLeaderboard();
        googleSheetAPI.onPostResult -= OnScoreSubmitted;
    }

    private void DisplayScores()
    {
        textPoints.text = "";
        if (onlineScores == null || onlineScores.Length == 0)
        {
            textPoints.text = "Loading leaderboard...";
            return;
        }

        for (int i = 0; i < onlineScores.Length; i++)
        {
            textPoints.text += $"{i + 1}. {onlineScores[i].name} - {onlineScores[i].score}\n";
        }

        buttonRetry.SetActive(true);
        buttonMenu.SetActive(true);
    }

    // ✅ Button functions
    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // Debug score for testing
    public void ForceDebugScore(int score)
    {
        Debug.Log("HighScoreManager: Debug score forced: " + score);

        pendingScore = score;
        nameEntryPanel.SetActive(true);
        textPoints.text = "DEBUG HIGHSCORE!\nEnter your name:";
    }
}
