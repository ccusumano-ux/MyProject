using UnityEngine;
using System.Collections.Generic;

public class MainMenuLeaderboard : MonoBehaviour
{
    [Header("Google Sheets API Reference")]
    public GoogleSheetsAPI googleSheetsAPI;

    [Header("Settings")]
    public bool autoFetchOnStart = true;

    // ✅ Static cache accessible globally
    public static List<GoogleSheetsAPI.ScoreData> cachedLeaderboard = new List<GoogleSheetsAPI.ScoreData>();

    private void Start()
    {
        if (autoFetchOnStart && googleSheetsAPI != null)
        {
            FetchLeaderboard();
        }
    }

    public void FetchLeaderboard()
    {
        if (googleSheetsAPI != null)
        {
            googleSheetsAPI.onGetLeaderboard += OnLeaderboardReceived;
            googleSheetsAPI.GetLeaderboard();
        }
        else
        {
            Debug.LogWarning("MainMenuLeaderboard: GoogleSheetsAPI reference is missing.");
        }
    }

    private void OnLeaderboardReceived(string json)
    {
        // Unsubscribe immediately to avoid multiple callbacks
        googleSheetsAPI.onGetLeaderboard -= OnLeaderboardReceived;

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("MainMenuLeaderboard: Received empty JSON.");
            return;
        }

        try
        {
            var allScores = JsonHelper.FromJson<GoogleSheetsAPI.ScoreData>(json);

            // Skip the first row (header) if needed
            cachedLeaderboard.Clear();
            for (int i = 0; i < allScores.Length; i++)
            {
                cachedLeaderboard.Add(allScores[i]);
            }

            Debug.Log($"MainMenuLeaderboard: Cached {cachedLeaderboard.Count} entries.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"MainMenuLeaderboard: Failed to parse leaderboard JSON: {ex.Message}");
        }
    }
}
