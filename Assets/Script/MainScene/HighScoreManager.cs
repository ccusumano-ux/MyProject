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
    bool hasLogged = false;

    private void Start()
    {
        if (googleSheetAPI == null)
        {
            Debug.LogError("GoogleSheetAPI not assigned!");
            return;
        }

        googleSheetAPI.onGetLeaderboard += OnLeaderboardReceived;
        googleSheetAPI.GetLeaderboard();

        if (nameInputField != null)
            nameInputField.onSubmit.AddListener(_ => ConfirmName());
    }

    private void OnLeaderboardReceived(string json)
    {
        if (!hasLogged)
        {
            Debug.Log("Received leaderboard data: " + json);
            hasLogged = true;
        }

        if (string.IsNullOrEmpty(json)) return;

        try
        {
            onlineScores = JsonHelper.FromJson<GoogleSheetsAPI.ScoreData>(json)
                            .OrderByDescending(s => s.score)
                            .Take(5)
                            .ToArray();
            DisplayScores();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse leaderboard JSON: " + e.Message);
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

        isNewHighScore = onlineScores.Length < 5 || finalScore > onlineScores.Min(s => s.score);

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

    public void ForceDebugScore(int finalScore)
    {
        deathCanvas.SetActive(true);
        darkPanel.SetActive(true);
        boxPanel.SetActive(true);
        textYouDied.text = "YOU DIED";
        textHighScore.text = "HIGHSCORES";

        pendingScore = finalScore;

        if (onlineScores == null || onlineScores.Length < 5)
        {
            // Fill empty leaderboard
            onlineScores = new GoogleSheetsAPI.ScoreData[5];
            for (int i = 0; i < 5; i++)
            {
                onlineScores[i] = new GoogleSheetsAPI.ScoreData { name = "AAA", score = 0 };
            }
        }

        onlineScores[4].score = finalScore;
        onlineScores[4].name = "DEBUG";

        googleSheetAPI.PostScore("DEBUG", finalScore);

        DisplayScores();
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

    // 🔹 BUTTON: Retry current level
    public void OnRetryButton()
    {
        Time.timeScale = 1f; // resume game time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 🔹 BUTTON: Return to Main Menu
    public void OnMenuButton()
    {
        Time.timeScale = 1f; // resume time
        SceneManager.LoadScene("MainMenu"); // change to your actual menu scene name
    }
}
