using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using SimpleJSON;

public class SessionManger : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject buttonPrefab;         // Your Button Prefab
    public Transform buttonParent;          // Scroll View > Viewport > Content
    public Button loadSessionsButton;       // The button to trigger loading
    //public ScrollRect scrollRect;           // Optional: to auto-scroll
    public ModularChatbot chatbotScript; // Assign in Inspector


    private string apiUrl = "https://3cf5-102-43-159-143.ngrok-free.app/get_user_sessions"; // Replace with your API

    void Start()
    {
        loadSessionsButton.onClick.AddListener(() =>
        {
            StartCoroutine(LoadAndDisplaySessions());
        });
    }

    IEnumerator LoadAndDisplaySessions()
    {
        int userId = PlayerPrefs.GetInt("user_id", -1); // Default -1 if not set

        if (userId == -1)
        {
            Debug.LogError("User ID not found in PlayerPrefs.");
            yield break;
        }

        // 🟩 Prepare JSON body
        string jsonBody = "{\"user_id\":" + userId + "}";

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(" Failed to load sessions: " + request.error);
            yield break;
        }

        JSONNode json = JSON.Parse(request.downloadHandler.text);
        JSONArray sessions = json["session_info"].AsArray;


        foreach (JSONNode session in sessions)
        {
            string title = session["title"];
            string sessionId = session["session_id"];




            Debug.Log("Creating button for: " + title);
            if (buttonPrefab == null)
            {
                Debug.LogError("❌ buttonPrefab is NULL!");
            }
            if (buttonParent == null)
            {
                Debug.LogError("❌ buttonParent is NULL!");
            }

            GameObject buttonObj = Instantiate(buttonPrefab, buttonParent);
            Debug.Log("✅ Button instantiated: " + buttonObj.name);





            TMP_Text label = buttonObj.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = title;

            string capturedId = sessionId;
            string capturedTitle = title;

            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log($"Clicked: {capturedTitle} ({capturedId})");
                StartCoroutine(LoadChatHistory(capturedId));

            

            });
        }

        Canvas.ForceUpdateCanvases();
        
    }










    IEnumerator LoadChatHistory(string sessionId)
    {
        string historyApi = "https://3cf5-102-43-159-143.ngrok-free.app/get_chat_history"; // Update if needed

        string jsonBody = "{\"session_id\":\"" + sessionId + "\"}";
        UnityWebRequest request = new UnityWebRequest(historyApi, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Failed to load chat history: " + request.error);
            yield break;
        }

        JSONNode json = JSON.Parse(request.downloadHandler.text);
        JSONArray chatHistory = json["chat_history"].AsArray;
        string sessionIdReturned = json["session_id"];

        chatbotScript.LoadChatHistory(chatHistory, sessionIdReturned);
    }


}