using UnityEngine;
using UnityEngine.UI;

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

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void StartTimer()
    {
        TimeRemaining = timeLimit;
        IsTimerRunning = true;
    }

    public void StopTimer()
    {
        IsTimerRunning = false;
    }

    public void PauseTimer() => IsTimerRunning = false;
    public void ResumeTimer() => IsTimerRunning = true;

    [SerializeField] private Text timerText; // 시간 표시용 텍스트 (민채은 수정)

    void Update()
    {
        if (!IsTimerRunning) return;
        TimeRemaining -= Time.deltaTime;

        // 타이머 UI 업데이트 (민채은 수정)
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(TimeRemaining) + "초";

        if (TimeRemaining <= 0)
        {
            TimeRemaining = 0;
            StopTimer();
            GameManager.GetInstance().OnGameOver(); // 시간 초과 시 게임오버
        }
    }
}