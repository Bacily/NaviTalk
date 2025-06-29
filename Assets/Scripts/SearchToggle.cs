using UnityEngine;
using TMPro;

public class SearchToggle : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField input_search;
    public void ToggleObject()
    {
        bool isActive = input_search.gameObject.activeSelf;

        if (isActive)
        {
            input_search.text = ""; // Clear text before hiding
        }

        input_search.gameObject.SetActive(!isActive); // Toggle visibility
    }
}
