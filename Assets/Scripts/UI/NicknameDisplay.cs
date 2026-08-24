using UnityEngine;
using TMPro;

public class NicknameDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text nicknameText;

    void OnEnable()
    {
        SettingsManager.Instance.OnNicknameChanged += UpdateText;
        UpdateText(SettingsManager.Instance.Nickname); // 켜질 때 현재값으로 초기화
    }

    void OnDisable()
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.OnNicknameChanged -= UpdateText;
    }

    private void UpdateText(string nickname)
    {
        nicknameText.text = nickname;
    }
}