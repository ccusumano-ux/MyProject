using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GoogleSheetAPI : MonoBehaviour
{
    [Header("Web App URL (Version 2)")]
    public string googleSheetURL = "https://script.google.com/macros/s/AKfycbzEGWdCMCLRCVwYjtFD1BVWGPxyfnlmP3CsHUbyLXyahZ3hTrDLk5jaVWUcf60DuXAj/exec";

    // Serializable class for JSON data
    [System.Serializable]
    public class ScoreData
    {
        public string name;
        public int score;
    }

    // Public method to call from HighScoreManager
    public void SubmitScore(string playerName, int playerScore)
    {
        StartCoroutine(PostScore(playerName, playerScore));
    }

    // Coroutine to handle POST request
    private IEnumerator PostScore(string playerName, int playerScore)
    {
        ScoreData data = new ScoreData { name = playerName, score = playerScore };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest www = new UnityWebRequest(googleSheetURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Score submitted successfully!");
            Debug.Log("Server Response: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ Error submitting score: " + www.error);
            Debug.LogError("Server Response: " + www.downloadHandler.text);
        }
    }
}
