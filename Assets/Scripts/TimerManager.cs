using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }
    public static TimerManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("TimerManager");
            obj.AddComponent<TimerManager>();
        }
        return Instance;
    }
    public float timeLimit = 60f;
    public float TimeRemaining { get; private set; }
    public bool IsTimerRunning { get; private set; }
    [SerializeField] private Text timerText; // 시간 표시용 텍스트 (씬마다 다름)
    [SerializeField] private Image timerBar; // 타이머 바 이미지 (씬마다 다름)

    // 타이머를 사용하지 않는 씬 목록 (GameManager와 연동하지 않아주세요)
    [SerializeField]
    private string[] scenesWithoutTimer = { "StartScene", "StageSelectScene" };

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 등록
    }

    // 현재 씬이 타이머를 사용하지 않는 씬인지 확인
    private bool IsTimerNeeded(string sceneName)
    {
        foreach (string name in scenesWithoutTimer)
        {
            if (sceneName == name)
                return false;
        }
        return true;
    }

    public void StartTimer()
    {
        if (!IsTimerNeeded(SceneManager.GetActiveScene().name))
        {
            Debug.LogWarning("[TimerManager] 타이머가 필요 없는 씬이라 StartTimer를 무시합니다: " + SceneManager.GetActiveScene().name);
            return;
        }

        if (timerText == null)
        {
            GameObject textObj = GameObject.Find("TimerText");
            if (textObj != null)
                timerText = textObj.GetComponent<Text>();
        }
        if (timerBar == null)
        {
            GameObject barObj = GameObject.Find("TimerBar");
            if (barObj != null)
                timerBar = barObj.GetComponent<Image>();
        }

        if (timerText != null)
            timerText.gameObject.SetActive(true);
        if (timerBar != null)
            timerBar.gameObject.SetActive(true);

        TimeRemaining = timeLimit;
        IsTimerRunning = true;
    }

    public void StopTimer()
    {
        IsTimerRunning = false;
    }
    public void PauseTimer() => IsTimerRunning = false;
    public void ResumeTimer() => IsTimerRunning = true;

    void Update()
    {
        if (!IsTimerRunning) return;
        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining <= 0)
            TimeRemaining = 0;

        // UI 업데이트
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(TimeRemaining / 60f);
            int seconds = Mathf.FloorToInt(TimeRemaining % 60f);
            int milliseconds = Mathf.FloorToInt((TimeRemaining * 100f) % 100f);
            timerText.text = string.Format("남은 접속 시간 {0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        }
        UpdateTimerBar();

        // 타이머 값이 0 이하가 되면 게임 오버 처리
        if (TimeRemaining <= 0)
        {
            StopTimer();
            GameManager.GetInstance().OnGameOver();
        }
    }

    // 남은 시간에 따라 타이머 바의 색상 변화 (씬마다 다름)
    private void UpdateTimerBar()
    {
        if (timerBar == null) return; // 방어 코드

        timerBar.fillAmount = TimeRemaining / timeLimit;
        if (TimeRemaining <= 15f)
        {
            timerBar.color = new Color32(255, 61, 0, 255);
            BGMManager.GetInstance().PlayBGM("stage0_fast"); // 남은 시간 얼마 없을 때 브금 전환 (Stage0 공용)
        }
        else if (TimeRemaining <= 30f)
            timerBar.color = new Color32(255, 160, 0, 255);
        else
            timerBar.color = new Color32(29, 233, 182, 255);
    }

    // 시간 감소 함수 (씬마다 다름)
    public void ReduceTime(float amount)
    {
        TimeRemaining = Mathf.Max(0, TimeRemaining - amount);
    }

    // 씬 로드될 때마다 타이머 초기화 (씬마다 다름)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopTimer();

        // 타이머가 필요 없는 씬이면 UI 감추고 종료
        if (!IsTimerNeeded(scene.name))
        {
            if (timerText != null)
                timerText.gameObject.SetActive(false);
            if (timerBar != null)
                timerBar.gameObject.SetActive(false);

            timerText = null;
            timerBar = null;
            return;
        }

        // 씬 진입할 때 timerText 다시 찾기
        GameObject textObj = GameObject.Find("TimerText");
        if (textObj != null)
            timerText = textObj.GetComponent<Text>();

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        GameObject barObj = GameObject.Find("TimerBar");
        if (barObj != null)
            timerBar = barObj.GetComponent<Image>();
        if (timerBar != null)
            timerBar.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트 해제
    }
}
