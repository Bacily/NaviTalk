using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using SimpleJSON;
using System.Text.RegularExpressions;

public class ModularChatbot : MonoBehaviour
{
    public TMP_InputField userInputField;
    public TextMeshProUGUI chatHistoryText;
    public ScrollRect scrollRect;
    public ScrollRect scrollSession;

    private string chatbotEndpoint = "https://3cf5-102-43-159-143.ngrok-free.app/ModularChatBot";

    private int userId = -1;
    private string sessionId = null;
    private bool isFirstMessage = true;

    void Start()
    {
        userId = PlayerPrefs.GetInt("user_id", -1);
        if (userId == -1)
        {
            chatHistoryText.text += "\nMego: You must log in before using the chatbot.\n";
            userInputField.interactable = false;
        }
    }

    public void SendMessageToChatbot()
    {
        string userMessage = userInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            string coloredSender = GetColoredSender("User");
            chatHistoryText.text += $"\n{coloredSender}: {userMessage}\n";
            StartCoroutine(GetChatbotResponse(userMessage));
            userInputField.text = "";
        }
        else
        {
            chatHistoryText.text += "\nMego: Please enter a message.\n";
            ScrollToBottom();
        }
    }

    IEnumerator GetChatbotResponse(string message)
    {
        // Build payload JSON
        JSONObject jsonPayload = new JSONObject();
        jsonPayload["user_id"] = userId;
        jsonPayload["question"] = message;


        if (isFirstMessage) {
            string shortTitle = message.Length > 28 ? message.Substring(0, 28) + "..." : message;

            jsonPayload["title"] = shortTitle;
        }
        

        if (!isFirstMessage && !string.IsNullOrEmpty(sessionId))
        {
            jsonPayload["session_id"] = sessionId;
        }

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload.ToString());

        using (UnityWebRequest request = new UnityWebRequest(chatbotEndpoint, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();



            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("Raw Response: " + responseText);

                var json = JSON.Parse(responseText);
                string answer = json["answer"];
                string newSessionId = json["session_id"];

                if (!string.IsNullOrEmpty(newSessionId))
                {
                    sessionId = newSessionId;
                    isFirstMessage = false;
                }

                string formattedAnswer = ConvertMarkdownToTMP(answer);

                string coloredSender = GetColoredSender("Mego");
                chatHistoryText.text += $"\n{coloredSender}: {formattedAnswer}\n";


                //chatHistoryText.text += "\nMego: " + (string.IsNullOrEmpty(formattedAnswer) ? "No response received." : formattedAnswer) + "\n";
                ScrollToBottom();
            }
            else
            {
                chatHistoryText.text += $"\nMego: Error: {request.error}\n";
                Debug.LogError($"Request failed: {request.error}, Code: {request.responseCode}, Response: {request.downloadHandler.text}");
                ScrollToBottom();
            }
        }
    }

    public static string ConvertMarkdownToTMP(string markdown)
    {
        if (string.IsNullOrEmpty(markdown)) return "";

        string converted = markdown;

        converted = Regex.Replace(converted, @"\*\*(.*?)\*\*", "<b>$1</b>");
        converted = Regex.Replace(converted, @"(?<!\*)\*(?!\*)(.*?)(?<!\*)\*(?!\*)", "<i>$1</i>");
        converted = Regex.Replace(converted, @"_(.*?)_", "<i>$1</i>");
        converted = Regex.Replace(converted, @"`(.*?)`", "<color=#444444><font=\"Courier New\">$1</font></color>");
        converted = Regex.Replace(converted, @"\[(.*?)\]\((.*?)\)", "<u>$1</u>");
        converted = Regex.Replace(converted, @"^# (.*?)$", "<size=140%><b>$1</b></size>", RegexOptions.Multiline);
        converted = Regex.Replace(converted, @"^## (.*?)$", "<size=120%><b>$1</b></size>", RegexOptions.Multiline);
        converted = Regex.Replace(converted, @"^### (.*?)$", "<size=110%><b>$1</b></size>", RegexOptions.Multiline);
        converted = Regex.Replace(converted, @"^[-*] (.*?)$", "• $1", RegexOptions.Multiline);
        converted = Regex.Replace(converted, @"^> (.*?)$", "<color=#888888>$1</color>", RegexOptions.Multiline);
        converted = Regex.Replace(converted, @"```(.*?)```", "<color=#444444><font=\"Courier New\">$1</font></color>", RegexOptions.Singleline);
        converted = Regex.Replace(converted, @"[\*_]{1,3}(?!\w)", "");
        converted = Regex.Replace(converted, @"#+", "");
        converted = Regex.Replace(converted, @">[>]+\s?", "");
        converted = Regex.Replace(converted, @"`+", "");
        converted = Regex.Replace(converted, @"\n+", "\n");

        return converted;
    }

    void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void ToggleMenu()
    {
        bool isSessionVisible = scrollSession.gameObject.activeSelf;
        scrollSession.gameObject.SetActive(!isSessionVisible);
        scrollRect.gameObject.SetActive(isSessionVisible);
        userInputField.gameObject.SetActive(isSessionVisible);
    }





    public void LoadChatHistory(JSONArray historyArray, string sessionIdFromServer)
    {
        ToggleMenu();

        this.sessionId = sessionIdFromServer;
        isFirstMessage = false;


        string groupedText = "";
        bool lastWasUser = false;

        foreach (JSONNode messageNode in historyArray)
        {
            string sender = messageNode["type"] == "human" ? "User" : "Mego";
            string message = ConvertMarkdownToTMP(messageNode["message"]);

            string senderColored = sender == "User"
                ? "<color=#00BFFF><b>User</b></color>"
                : "<color=#32CD32><b>Mego</b></color>";

            groupedText += $"{senderColored}: {message}\n\n";

            if (sender == "Mego" && lastWasUser)
            {
                groupedText += "<size=8>\n\n\n</size>"; // Extra spacing between Q&A
            }

            lastWasUser = sender == "User";
        }

        chatHistoryText.text = groupedText;
        ScrollToBottom();


    }
    private string GetColoredSender(string sender)
    {
        return sender == "User"
            ? "<color=#00BFFF><b>User</b></color>"   // Light Blue
            : "<color=#32CD32><b>Mego</b></color>";  // Lime Green
    }

    public void SetSessionId(string newSessionId)
    {
        sessionId = newSessionId;
        isFirstMessage = false; // prevent creating new session
    }




}
