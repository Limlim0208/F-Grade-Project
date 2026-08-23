using System;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] private GameObject settingsCanvas;

    // === 닉네임 상태 ===
    private const string NicknameKey = "USER_NICKNAME";
    public string Nickname { get; private set; } = "신입생";
    public event Action<string> OnNicknameChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Nickname = PlayerPrefs.GetString(NicknameKey, "신입생");
        }
        else Destroy(gameObject);
    }

    public static SettingsManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("SettingsManager");
            obj.AddComponent<SettingsManager>();
        }
        return Instance;
    }

    public void OpenSettings()
    {
        settingsCanvas.SetActive(true);
        if (GameManager.GetInstance().CurrentState == GameManager.GameState.Playing)
            GameManager.GetInstance().PauseGame();
    }

    public void CloseSettings()
    {
        settingsCanvas.SetActive(false);
        if (GameManager.GetInstance().CurrentState == GameManager.GameState.Paused)
            GameManager.GetInstance().ResumeGame();
    }

    // 닉네임 상태 관리
    public bool TrySetNickname(string newNickname, out string errorMessage)
    {
        newNickname = newNickname?.Trim();

        if (string.IsNullOrEmpty(newNickname))
        {
            errorMessage = "닉네임을 입력해주세요.";
            return false;
        }
        if (newNickname.Length > 8)
        {
            errorMessage = "닉네임은 자 이하로 입력해주세요.";
            return false;
        }

        Nickname = newNickname;
        PlayerPrefs.SetString(NicknameKey, Nickname);
        PlayerPrefs.Save();

        OnNicknameChanged?.Invoke(Nickname);
        errorMessage = null;
        return true;

    }
}