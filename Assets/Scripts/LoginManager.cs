using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;




public class LoginManager : MonoBehaviour
{
    [Header("Login UI Elements")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public UnityEngine.UI.Button loginButton;
    public TMP_Text feedbackText;

    private string loginApiUrl = "http://localhost:8080/api/auth/login"; // Update if hosted

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
    }

    void OnLoginClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            feedbackText.text = "❗ Enter both username and password.";
            return;
        }

        StartCoroutine(LoginUser(username, password));
    }

    IEnumerator LoginUser(string username, string password)
    {
        string jsonData = JsonUtility.ToJson(new LoginDto(username, password));

        UnityWebRequest request = new UnityWebRequest(loginApiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success || request.responseCode == 200)
        {
            string json = request.downloadHandler.text;

            AuthResponseDto response = JsonUtility.FromJson<AuthResponseDto>(json);

            Debug.Log(" Login successful. Token: " + response.token);
            feedbackText.text = $" Welcome, {response.userid}";

            PlayerPrefs.SetString("auth_token", response.token);
            PlayerPrefs.SetInt("user_id", (int)response.userid); // ✅ Save user ID
            PlayerPrefs.Save(); // Optional, but ensures data is written immediately
            

            // Optional: load another scene after login
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log(" Login failed: " + request.downloadHandler.text);
            feedbackText.text = " Login failed: Invalid credentials";
        }
    }

    [System.Serializable]
    public class LoginDto
    {
        public string username;
        public string password;

        public LoginDto(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }

    [System.Serializable]
    public class AuthResponseDto
    {
        public string token;
        public long userid;
    }
}
