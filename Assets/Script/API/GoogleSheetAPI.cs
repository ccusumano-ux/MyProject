using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class GoogleSheetsAPI : MonoBehaviour
{
    [Header("Main Google Apps Script URL")]
    [Tooltip("The original Apps Script web app endpoint (your Google Apps Script URL).")]
    public string googleSheetURL = "https://script.google.com/macros/s/AKfycbw288A9L3PuzMZ9pYOf8XszjiHfNgy6Ph9boNj0obyVFsxhbM_obaIesssHDGlnt-lf/exec";

    [Header("Cloudflare Worker Proxy URL")]
    [Tooltip("Proxy through Cloudflare Worker to handle CORS.")]
    public string cloudflareWorkerURL = "https://round-cell-4d63.carcatpsl.workers.dev/";

    [Header("Timeout (seconds)")]
    public float requestTimeout = 10f;

    public Action<string> onGetLeaderboard; // JSON string of leaderboard data
    public Action<bool> onPostResult;       // success/fail callback

    // --------------------------
    // GET Leaderboard
    // --------------------------
    public void GetLeaderboard()
    {
        StartCoroutine(GetRequest());
    }

    private IEnumerator GetRequest()
    {
        string url = $"{cloudflareWorkerURL}?url={UnityWebRequest.EscapeURL(googleSheetURL)}";
        Debug.Log("🌐 Sending GET request → " + url);

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = Mathf.RoundToInt(requestTimeout);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ GET success! Received data:\n" + www.downloadHandler.text);
            onGetLeaderboard?.Invoke(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ GET leaderboard failed: " + www.error);
            onGetLeaderboard?.Invoke(null);
        }
    }

    // --------------------------
    // POST New Score
    // --------------------------
    public void PostScore(string playerName, int score)
    {
        StartCoroutine(PostRequest(playerName, score));
    }

    private IEnumerator PostRequest(string playerName, int score)
    {
        ScoreData data = new ScoreData { name = playerName, score = score };
        string json = JsonUtility.ToJson(data);

        string url = $"{cloudflareWorkerURL}?url={UnityWebRequest.EscapeURL(googleSheetURL)}";
        Debug.Log($"📤 Sending POST request to {url} with data: {json}");

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        www.timeout = Mathf.RoundToInt(requestTimeout);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ POST success! Response: " + www.downloadHandler.text);
            onPostResult?.Invoke(true);
        }
        else
        {
            Debug.LogError("❌ POST score failed: " + www.error);
            onPostResult?.Invoke(false);
        }
    }

    [Serializable]
    public class ScoreData
    {
        public string name;
        public int score;
    }
}
