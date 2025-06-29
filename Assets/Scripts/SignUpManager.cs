using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.PackageManager;



[System.Serializable]
public class DetailedErrorResponse
{
    public string path;
    public string error;
    public string message;
    public string timestamp;
    public int status;
}

public class SignUpManager : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    [Header("UI Elements")]
    public TMP_Text feedbackText;
    public UnityEngine.UI.Button signUpButton;

    private string apiUrl = "http://localhost:8080/api/auth/register"; // Update this if you're deploying

    void Start()
    {
        signUpButton.onClick.AddListener(OnSignUpClicked);
    }

    void OnSignUpClicked()
    {
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;

        if (ValidateInput(username, email, password))
        {
            StartCoroutine(RegisterUser(username, email, password));
        }
        else
        {
            feedbackText.text = "Please fill all fields correctly.";
        }
    }

    IEnumerator RegisterUser(string username, string email, string password)
    {
        string jsonData = JsonUtility.ToJson(new RegisterDto(username, email, password));

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success || request.responseCode == 201)
        {
            Debug.Log("✅ User registered successfully!");
            feedbackText.text = "✅ Sign up successful!";

            SceneManager.LoadScene(4);
        }
        else
        {
            try
            {
                feedbackText.text = $"Username or Email Used!";
            }
            catch
            {
                feedbackText.text = "❌ An unexpected error occurred.";
            }
        }
    }

    bool ValidateInput(string username, string email, string password)
    {
        return !(string.IsNullOrWhiteSpace(username) ||
                 string.IsNullOrWhiteSpace(email) ||
                 string.IsNullOrWhiteSpace(password) ||
                 !email.Contains("@") ||
                 password.Length < 6);
    }

    [System.Serializable]
    public class RegisterDto
    {
        public string username;
        public string email;
        public string password;

        public RegisterDto(string username, string email, string password)
        {
            this.username = username;
            this.email = email;
            this.password = password;
        }
    }
}
