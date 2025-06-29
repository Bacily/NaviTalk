using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine.Networking;
using TMPro; // Add this for TextMeshPro

public class PostManager : MonoBehaviour
{
    [SerializeField] private GameObject postPrefab;
    [SerializeField] private Transform contentHolder;

    private string apiUrl = "http://ec2-13-51-175-113.eu-north-1.compute.amazonaws.com:8080/announcements";

    private void Start()
    {
        if (postPrefab == null)
        {
            Debug.LogError("PostPrefab is not assigned in Inspector!");
            return;
        }
        if (contentHolder == null)
        {
            Debug.LogError("ContentHolder is not assigned in Inspector!");
            return;
        }
        StartCoroutine(FetchAnnouncements());
    }

    private IEnumerator FetchAnnouncements()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching announcements: {webRequest.error}");
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log($"Raw JSON Response: {jsonResponse}");

                try
                {
                    JArray announcements = JArray.Parse(jsonResponse);
                    Debug.Log($"Parsed {announcements.Count} announcements");

                    foreach (var announcement in announcements)
                    {
                        string content = announcement["content"]?.ToString() ?? "No content available";
                        string date = announcement["date"]?.ToString() ?? "No date available";
                        if (!string.IsNullOrEmpty(date) && date.Contains("T"))
                        {
                            date = date.Split('T')[0]; // Extracts "2025-04-03"
                        }
                        Debug.Log($"Creating post: {content} - {date}");
                        CreatePost(content, date);
                    }
                    Debug.Log("Finished creating all posts");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error in FetchAnnouncements: {e.Message}\nStackTrace: {e.StackTrace}");
                }
            }
        }
    }

    private void CreatePost(string content, string date)
    {
        if (postPrefab == null)
        {
            Debug.LogError("postPrefab is null during CreatePost!");
            return;
        }
        if (contentHolder == null)
        {
            Debug.LogError("contentHolder is null during CreatePost!");
            return;
        }

        GameObject newPost = Instantiate(postPrefab, contentHolder);
        if (newPost == null)
        {
            Debug.LogError("Failed to instantiate postPrefab!");
            return;
        }

        Transform contentText = newPost.transform.Find("ContentText");
        if (contentText == null)
        {
            Debug.LogError("ContentText not found in post prefab!");
            return;
        }

        TMP_Text contentTextComponent = contentText.GetComponent<TMP_Text>(); // Use TMP_Text instead of Text
        if (contentTextComponent == null)
        {
            Debug.LogError("ContentText GameObject is missing a TMP_Text component!");
            return;
        }
        contentTextComponent.text = content;

        Transform dateText = newPost.transform.Find("DateText");
        if (dateText == null)
        {
            Debug.LogError("DateText not found in post prefab!");
            return;
        }

        TMP_Text dateTextComponent = dateText.GetComponent<TMP_Text>(); // Use TMP_Text instead of Text
        if (dateTextComponent == null)
        {
            Debug.LogError("DateText GameObject is missing a TMP_Text component!");
            return;
        }
        dateTextComponent.text = date;

        newPost.SetActive(true);
        Debug.Log($"Post created successfully: {content} - {date}");
    }
}