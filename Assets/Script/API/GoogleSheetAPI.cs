using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class GoogleSheetsAPI : MonoBehaviour
{
    [Header("Web App URL")]
    public string googleSheetURL = "https://script.google.com/macros/s/AKfycbw288A9L3PuzMZ9pYOf8XszjiHfNgy6Ph9boNj0obyVFsxhbM_obaIesssHDGlnt-lf/exec";

    [Header("Timeout")]
    public float requestTimeout = 10f;

    public Action<string> onGetLeaderboard; // callback with JSON string
    public Action<bool> onPostResult;       // callback: success/fail

    public void GetLeaderboard()
    {
        StartCoroutine(GetRequest());
    }

    private IEnumerator GetRequest()
    {
        string url = "https://api.allorigins.win/raw?url=" + UnityWebRequest.EscapeURL(googleSheetURL);

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = Mathf.RoundToInt(requestTimeout);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            onGetLeaderboard?.Invoke(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ GET leaderboard failed: " + www.error);
            onGetLeaderboard?.Invoke(null);
        }
    }

    public void PostScore(string playerName, int score)
    {
        StartCoroutine(PostRequest(playerName, score));
    }

    private IEnumerator PostRequest(string playerName, int score)
    {
        ScoreData data = new ScoreData { name = playerName, score = score };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest www = new UnityWebRequest(googleSheetURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            onPostResult?.Invoke(true);
            Debug.Log("✅ Score posted successfully!");
        }
        else
        {
            onPostResult?.Invoke(false);
            Debug.LogError("❌ POST score failed: " + www.error);
        }
    }

    [Serializable]
    public class ScoreData
    {
        public string name;
        public int score;
    }

}
//Version 0001