using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardHandler : MonoBehaviour
{
    public RectTransform chatInputField; // Assign your ChatInputField in the Inspector
    public RectTransform chatPanel; // Assign the parent panel that should move up
    private Vector2 originalPanelPosition;
    private TouchScreenKeyboard keyboard;

    void Start()
    {
        originalPanelPosition = chatPanel.anchoredPosition; // Store the original position
    }

    void Update()
    {
        if (keyboard != null)
        {
            if (keyboard.status == TouchScreenKeyboard.Status.Done || keyboard.status == TouchScreenKeyboard.Status.Canceled)
            {
                ResetUI();
            }
        }
    }

    public void OnInputFieldSelected()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        StartCoroutine(AdjustForKeyboard());
    }

    System.Collections.IEnumerator AdjustForKeyboard()
    {
        yield return new WaitForSeconds(0.1f); // Wait for the keyboard to show

        float keyboardHeight = GetKeyboardHeight();
        if (keyboardHeight > 0)
        {
            MoveUIUp(keyboardHeight);
        }
    }

    float GetKeyboardHeight()
    {
        return TouchScreenKeyboard.visible ? (Screen.height * 0.3f) : 0; // Estimated 30% of the screen
    }

    void MoveUIUp(float height)
    {
        chatPanel.anchoredPosition = new Vector2(chatPanel.anchoredPosition.x, originalPanelPosition.y + height);
    }

    public void ResetUI()
    {
        chatPanel.anchoredPosition = originalPanelPosition;
    }
}
