using UnityEngine;

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

    void Update()
    {
        if (!IsTimerRunning) return;
        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining <= 0)
        {
            TimeRemaining = 0;
            StopTimer();
            GameManager.GetInstance().OnGameOver(); // 시간 초과 시 게임오버
        }
    }
}