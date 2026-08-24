using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerClockManager : MonoBehaviour
{
    public static ServerClockManager Instance { get; private set; }
    public static ServerClockManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("ServerClockManager");
            obj.AddComponent<ServerClockManager>();
        }
        return Instance;
    }

    [Header("UI 참조")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image colorPanel;

    // 초기 표시 텍스트 (Stage 시작 직후, StartClock 호출 전)
     private string initialDisplayText = "2036년 02월 18일 00시 00분 00초";

    // 목표 시각 (수강신청 오픈 시각)
    private int targetYear = 2036;
    private int targetMonth = 2;
    private int targetDay = 15;
    private int targetHour = 9;
    private int targetMinute = 0;
    private int targetSecond = 0;

    [Header("랜덤 시작 범위 (목표 시각 기준 몇 초 전 ~ 몇 초 전)")]
    [Tooltip("예: 100~160 = 08:58:20 ~ 08:57:20 사이 랜덤 시작 (09:00:00 기준)")]
    [SerializeField] private float randomStartMinSecondsBeforeTarget = 100f;
    [SerializeField] private float randomStartMaxSecondsBeforeTarget = 160f;

    // "경고 색상 그라데이션
    private float warningStartSecondsBeforeTarget = 10f; // 목표 10초 전부터 그라데이션 시작
    private float warningFadeOutDuration = 3f;           // 목표 이후 원상복귀까지 걸리는 시간
    private Color normalColor = Color.white;
    private Color warningColor = Color.red;

    public DateTime TargetTime { get; private set; }
    public DateTime CurrentServerTime { get; private set; }
    public bool IsRegistrationOpen { get; private set; }

    /// 09:00:00 도달 시 단 한 번만 호출됨 (수강신청 오픈 트리거)
    public event Action OnRegistrationOpened;
    /// 매 프레임 현재 서버 시간을 전달 (과목별 마감시간 판정 등에 사용)
    public event Action<DateTime> OnTick;

    private DateTime startTime;
    private float elapsed;
    private bool isRunning;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameManager.GameState previousState, GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Playing && previousState != GameManager.GameState.Paused)
        {
            StartClock();
        }
    }

    void Start()
    {
        if (timerText != null)
            timerText.text = initialDisplayText;

        if (colorPanel != null)
            colorPanel.color = normalColor;
    }

    /// Stage 시작 시 StageManager 등에서 호출
    public void StartClock()
    {
        TargetTime = new DateTime(targetYear, targetMonth, targetDay, targetHour, targetMinute, targetSecond);

        float randomOffset = UnityEngine.Random.Range(randomStartMinSecondsBeforeTarget, randomStartMaxSecondsBeforeTarget);
        startTime = TargetTime.AddSeconds(-randomOffset);

        elapsed = 0f;
        IsRegistrationOpen = false;
        isRunning = true;

        CurrentServerTime = startTime;
        UpdateTimerText();
        UpdateColorPanel();
    }

    void Update()
    {
        if (!isRunning) return;

        elapsed += Time.deltaTime;
        CurrentServerTime = startTime.AddSeconds(elapsed);

        UpdateTimerText();
        UpdateColorPanel();

        OnTick?.Invoke(CurrentServerTime);

        if (!IsRegistrationOpen && CurrentServerTime >= TargetTime)
        {
            IsRegistrationOpen = true;
            OnRegistrationOpened?.Invoke();
        }
    }

    private void UpdateTimerText()
    {
        if (timerText == null) return;

        timerText.text = $"{CurrentServerTime.Year}년 {CurrentServerTime.Month:00}월 {CurrentServerTime.Day:00}일 " +
                          $"{CurrentServerTime.Hour:00}시 {CurrentServerTime.Minute:00}분 {CurrentServerTime.Second:00}초";
    }

    private void UpdateColorPanel()
    {
        if (colorPanel == null) return;

        double secondsUntilTarget = (TargetTime - CurrentServerTime).TotalSeconds;

        if (secondsUntilTarget > 0 && secondsUntilTarget <= warningStartSecondsBeforeTarget)
        {
            // 목표 시각 10초 전 ~ 목표 시각: 하양 → 빨강
            float t = 1f - (float)(secondsUntilTarget / warningStartSecondsBeforeTarget);
            colorPanel.color = Color.Lerp(normalColor, warningColor, t);
        }
        else if (secondsUntilTarget <= 0)
        {
            // 목표 시각 이후: 빨강 → 하양 (warningFadeOutDuration 동안)
            float elapsedAfterTarget = (float)(-secondsUntilTarget);
            float t = Mathf.Clamp01(elapsedAfterTarget / warningFadeOutDuration);
            colorPanel.color = Color.Lerp(warningColor, normalColor, t);
        }
        else
        {
            colorPanel.color = normalColor;
        }
    }

    /// 목표 시각 기준 경과 초 (오픈 전이면 음수). 과목별 마감시간 판정에 사용
    public float SecondsSinceRegistrationOpened => (float)(CurrentServerTime - TargetTime).TotalSeconds;
}