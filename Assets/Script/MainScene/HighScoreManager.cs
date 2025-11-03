using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class HighScoreManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject deathCanvas;
    public GameObject darkPanel;
    public GameObject boxPanel;

    public TextMeshProUGUI textYouDied;
    public TextMeshProUGUI textHighScore;
    public TextMeshProUGUI textPoints;
    public TMP_InputField nameInputField; // new
    public GameObject nameEntryPanel;     // small panel with "Enter Name" + InputField + Confirm button
    public GameObject buttonRetry;
    public GameObject buttonMenu;

    [Header("Online Score API")]
    public GoogleSheetAPI googleSheetAPI; // assign in Inspector


    [Header("Base High Scores")]
    public int[] baseScores = { 500, 400, 300, 200, 100 };

    private const string HighScoreKey = "HighScores";
    private const string NameKey = "HighScoreNames";

    private List<int> highScores = new List<int>();
    private List<string> highScoreNames = new List<string>();

    private int pendingScore = 0;
    private bool isNewHighScore = false;

    private void Start()
    {
        LoadHighScores();
        HidePanel();

        if (nameInputField != null)
            nameInputField.onSubmit.AddListener(_ => ConfirmName());
    }

    public void ShowPanel(int finalScore)
    {
        deathCanvas.SetActive(true);
        darkPanel.SetActive(true);
        boxPanel.SetActive(true);
        textYouDied.text = "YOU DIED";
        textHighScore.text = "HIGHSCORES";
        

        pendingScore = finalScore;

        // Check if it qualifies for top 5
        isNewHighScore = highScores.Count < 5 || finalScore > highScores.Min();

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

        if (string.IsNullOrEmpty(playerName))
            playerName = "AAA";

        if (playerName.Length > 6)
            playerName = playerName.Substring(0, 6);

        AddNewScore(pendingScore, playerName);
        SaveHighScores();

        // --- Send score to Google Sheets ---
        if (googleSheetAPI != null)
            googleSheetAPI.SubmitScore(playerName, pendingScore);

        nameEntryPanel.SetActive(false);
        DisplayScores();
    }

    private void DisplayScores()
    {
        textPoints.text = "";
        for (int i = 0; i < highScores.Count; i++)
        {
            textPoints.text += $"{i + 1}. {highScoreNames[i]}  -  {highScores[i]}\n";
        }

        buttonRetry.SetActive(true);
        buttonMenu.SetActive(true);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void HidePanel()
    {
        deathCanvas.SetActive(false);
    }

    private void AddNewScore(int score, string name)
    {
        highScores.Add(score);
        highScoreNames.Add(name);

        // Order by score
        var combined = highScores
            .Select((s, i) => new { Score = s, Name = highScoreNames[i] })
            .OrderByDescending(x => x.Score)
            .Take(5)
            .ToList();

        highScores = combined.Select(x => x.Score).ToList();
        highScoreNames = combined.Select(x => x.Name).ToList();
    }

    private void SaveHighScores()
    {
        PlayerPrefs.SetString(HighScoreKey, string.Join(",", highScores));
        PlayerPrefs.SetString(NameKey, string.Join(",", highScoreNames));
        PlayerPrefs.Save();
    }

    private void LoadHighScores()
    {
        if (PlayerPrefs.HasKey(HighScoreKey) && PlayerPrefs.HasKey(NameKey))
        {
            highScores = PlayerPrefs.GetString(HighScoreKey).Split(',').Select(int.Parse).ToList();
            highScoreNames = PlayerPrefs.GetString(NameKey).Split(',').ToList();
        }
        else
        {
            // Load defaults
            highScores = baseScores.ToList();
            highScoreNames = new List<string> { "AAA", "BBB", "CCC", "DDD", "EEE" };
        }
    }
}
