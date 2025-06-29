using UnityEngine;
using UnityEngine.UI;

public class ToggleImage : MonoBehaviour
{
    public Image targetImage;        // Reference to the UI Image component
    public Sprite sprite1;           // First sprite
    public Sprite sprite2;           // Second sprite

    private bool usingFirstSprite = true;

    public void Toggle()
    {
        if (targetImage == null || sprite1 == null || sprite2 == null)
        {
            Debug.LogWarning("Image or sprites not assigned.");
            return;
        }

        if (usingFirstSprite)
            targetImage.sprite = sprite2;
        else
            targetImage.sprite = sprite1;

        usingFirstSprite = !usingFirstSprite;
    }
}
