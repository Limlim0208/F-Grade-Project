using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    // === 닉네임 상태 ===
    private const string NicknameKey = "USER_NICKNAME";
    public string Nickname { get; private set; } = "신입생";
    public event Action<string> OnNicknameChanged;

    // === 프로필 이미지 상태 ===
    [SerializeField] private Sprite[] availableProfileImages;
    private const string ProfileImageIndexKey = "PROFILE_IMAGE_INDEX";
    public int ProfileImageIndex { get; private set; } = 0;
    public Sprite CurrentProfileSprite =>
        (availableProfileImages != null && ProfileImageIndex < availableProfileImages.Length)
            ? availableProfileImages[ProfileImageIndex] : null;
    public Sprite[] AvailableProfileImages => availableProfileImages;
    public event Action<Sprite> OnProfileImageChanged;

    // === 볼륨 상태 (실제 저장/적용은 BGMManager / SFXManager에 위임) ===
    public float BGMVolume => BGMManager.GetInstance().CurrentBGMVolume;
    public float SFXVolume => SFXManager.GetInstance().CurrentSFXVolume;
    public event Action<float> OnBGMVolumeChanged;
    public event Action<float> OnSFXVolumeChanged;
    private const float DefaultVolume = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Nickname = PlayerPrefs.GetString(NicknameKey, "신입생");

            ProfileImageIndex = PlayerPrefs.GetInt(ProfileImageIndexKey, 0);
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

        RefreshVolumeSliders();
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

    // 프로필 사진 상태 관리
    public bool TrySetProfileImage(int index)
    {
        if (availableProfileImages == null || index < 0 || index >= availableProfileImages.Length)
            return false;

        ProfileImageIndex = index;
        PlayerPrefs.SetInt(ProfileImageIndexKey, index);
        PlayerPrefs.Save();

        OnProfileImageChanged?.Invoke(CurrentProfileSprite);
        return true;
    }

    // 볼륨 상태 관리 (슬라이더 onValueChanged에 직접 연결)
    public void SetBGMVolume(float volume)
    {
        BGMManager.GetInstance().SetVolume(volume);
        OnBGMVolumeChanged?.Invoke(volume);
    }

    public void SetSFXVolume(float volume)
    {
        SFXManager.GetInstance().SetVolume(volume);
        OnSFXVolumeChanged?.Invoke(volume);
    }


    // 팝업 열릴 때마다 현재 볼륨값으로 슬라이더 동기화 (콜백 중복 호출 방지)
    private void RefreshVolumeSliders()
    {
        Debug.Log($"[Settings] Refresh called. BGM={BGMVolume}, SFX={SFXVolume}, bgmSlider={(bgmSlider != null)}, sfxSlider={(sfxSlider != null)}");

        if (bgmSlider != null)
            bgmSlider.SetValueWithoutNotify(BGMVolume);

        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(SFXVolume);
    }

    // 볼륨 리셋버튼
    public void ResetBGMVolume()
    {
        SetBGMVolume(DefaultVolume);
        if (bgmSlider != null)
            bgmSlider.SetValueWithoutNotify(DefaultVolume);
    }

    public void ResetSFXVolume()
    {
        SetSFXVolume(DefaultVolume);
        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(DefaultVolume);
    }

}