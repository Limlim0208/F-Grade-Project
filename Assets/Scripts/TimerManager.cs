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
    [SerializeField] private Text timerText; // 시간 표시용 텍스트 (민채은 수정)
    [SerializeField] private Image timerBar; // 타이머 바 이미지 (민채은 수정)
    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 등록
    }
    void Start()
    {

        // 처음엔 타이머 텍스트, 바 숨김
        if (timerText != null)
            timerText.gameObject.SetActive(false);
        if (timerBar != null) 
            timerBar.gameObject.SetActive(false);
    }
    public void StartTimer()
    {
        // 타이머 시작 시 텍스트, 바 표시
        if (timerText != null)
            timerText.gameObject.SetActive(true);
        if (timerBar != null)
            timerBar.gameObject.SetActive(true);
        TimeRemaining = timeLimit;
        IsTimerRunning = true;

        IsTimerRunning = false; // [테스트] 타이머 자동 시작 방지
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

        if (TimeRemaining <= 0)
        {
            StopTimer();
            GameManager.GetInstance().OnGameOver(); // 타이머 시간 종료시 게임 오버
        }
    }

    // 남은 시간에 따른 타이머 바의 색상 변화 (민채은 수정)
    private void UpdateTimerBar()
    {
        timerBar.fillAmount = TimeRemaining / timeLimit;
        if (TimeRemaining <= 15f)
            timerBar.color = new Color32(255, 61, 0, 255); 
        else if (TimeRemaining <= 30f)
            timerBar.color = new Color32(255, 160, 0, 255); 
        else
            timerBar.color = new Color32(29, 233, 182, 255); 
    }

    // 시간 감소 함수 (민채은 수정)
    public void ReduceTime(float amount)
    {
        TimeRemaining = Mathf.Max(0, TimeRemaining - amount);
    }

    // 씬 로드될 때마다 타이머 초기화 (민채은 수정)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopTimer();
        // 새 씬에서 timerText 다시 찾기
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