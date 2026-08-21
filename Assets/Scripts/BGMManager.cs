using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }
    public static BGMManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("BGMManager");
            obj.AddComponent<BGMManager>();
        }
        return Instance;
    }

    private AudioSource audioSource;
    private string currentClipName;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 바뀌면 일단 정지, 새 씬에서 필요하면 그 씬의 매니저가 다시 PlayBGM 호출
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopBGM();
    }

    // clipName은 Resources/BGM/ 폴더 안의 파일 이름 (확장자 제외)
    public void PlayBGM(string clipName)
    {
        if (currentClipName == clipName && audioSource.isPlaying)
            return; // 이미 같은 곡이 재생 중이면 무시 (씬 전환마다 끊기지 않게)

        AudioClip clip = Resources.Load<AudioClip>($"BGM/{clipName}");
        if (clip == null)
        {
            Debug.LogWarning($"[BGMManager] BGM을 찾을 수 없습니다: {clipName}");
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
        currentClipName = clipName;
    }

    public void StopBGM()
    {
        audioSource.Stop();
        currentClipName = null;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    // 현재 재생 중인 곡의 재생 위치를 seconds만큼 앞으로 건너뜀
    public void SkipForward(float seconds)
    {
        if (audioSource.clip == null) return;
        audioSource.time = Mathf.Min(audioSource.time + seconds, audioSource.clip.length - 0.01f);
    }
}
