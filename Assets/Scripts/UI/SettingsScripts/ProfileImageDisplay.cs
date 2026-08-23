using UnityEngine;
using UnityEngine.UI;

public class ProfileImageDisplay : MonoBehaviour
{
    [SerializeField] private Image profileImage;

    void OnEnable()
    {
        SettingsManager.Instance.OnProfileImageChanged += UpdateImage;
        UpdateImage(SettingsManager.Instance.CurrentProfileSprite);
    }

    void OnDisable()
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.OnProfileImageChanged -= UpdateImage;
    }

    private void UpdateImage(Sprite sprite)
    {
        if (sprite != null)
            profileImage.sprite = sprite;
    }
}