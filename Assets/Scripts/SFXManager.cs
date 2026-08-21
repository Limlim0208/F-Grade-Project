using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }
    public static SFXManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("SFXManager");
            obj.AddComponent<SFXManager>();
        }
        return Instance;
    }

    private AudioSource sfxSource;
    private bool clickRequestedThisFrame = false;
    private bool suppressClickThisFrame = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            clickRequestedThisFrame = true;
    }

    private const float ClickVolumeScale = 0.8f;
    private const float ErrorVolumeScale = 1.2f;

    void LateUpdate()
    {
        // 같은 프레임에 다른 효과음(error 등)이 이미 재생됐으면 클릭음은 생략
        if (clickRequestedThisFrame && !suppressClickThisFrame)
            PlayClip("click", ClickVolumeScale);

        clickRequestedThisFrame = false;
        suppressClickThisFrame = false;
    }

    // clipName은 Resources/SFX/ 폴더 안의 파일 이름 (확장자 제외)
    // BGM과 별도 채널이라 재생 중인 BGM을 끊지 않고 겹쳐서 재생됨
    public void PlaySFX(string clipName)
    {
        if (clipName != "click")
            suppressClickThisFrame = true; // 이번 프레임 자동 클릭음은 생략

        float volumeScale = clipName == "error" ? ErrorVolumeScale : 1f;
        PlayClip(clipName, volumeScale);
    }

    private void PlayClip(string clipName, float volumeScale)
    {
        AudioClip clip = Resources.Load<AudioClip>($"SFX/{clipName}");
        if (clip == null)
        {
            Debug.LogWarning($"[SFXManager] SFX를 찾을 수 없습니다: {clipName}");
            return;
        }

        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void SetVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}
