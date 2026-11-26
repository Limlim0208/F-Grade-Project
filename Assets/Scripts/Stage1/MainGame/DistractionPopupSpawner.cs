using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DistractionPopupSpawner : MonoBehaviour
{
    [Header("팝업 프리팹 / 부모(Canvas)")]
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Transform popupParent;

    [Header("발생 구간 (메인게임 시작 ~ 9시 직전)")]
    [Tooltip("메인게임 시작 후 첫 팝업이 뜨기까지 최소 대기 시간(초)")]
    [SerializeField] private float startDelayAfterMainGameStart = 3f;
    [Tooltip("9시 몇 초 전까지만 팝업을 띄울지(초)")]
    [SerializeField] private float endSecondsBeforeTarget = 30f;

    [Header("발생 횟수 (랜덤)")]
    [SerializeField] private int minPopupCount = 7;
    [SerializeField] private int maxPopupCount = 10;

    [Header("팝업 스폰 위치 랜덤 범위 (anchoredPosition 기준)")]
    [SerializeField] private Vector2 randomPosMin = new Vector2(-300f, -200f);
    [SerializeField] private Vector2 randomPosMax = new Vector2(300f, 200f);

    [Header("효과음")]
    [SerializeField] private string popupSfxName = "error";

    // "9시까지 남은 시간" 기준 트리거 시점들 (내림차순: 큰 값 = 더 이른 시점)
    private List<float> scheduledTimes;
    private bool hasScheduled = false;

    private void OnEnable()
    {
        ServerClockManager.GetInstance().OnTick += HandleTick;
    }

    private void OnDisable()
    {
        if (ServerClockManager.Instance != null)
            ServerClockManager.Instance.OnTick -= HandleTick;
    }

    private void HandleTick(System.DateTime currentServerTime)
    {
        if (!MainGameManager.GetInstance().IsMainGameActive)
            return;

        var clock = ServerClockManager.GetInstance();
        double secondsUntilTarget = (clock.TargetTime - currentServerTime).TotalSeconds;

        // 메인게임이 시작된 뒤 첫 틱에 딱 한 번만 스케줄 생성
        if (!hasScheduled)
        {
            GenerateSchedule((float)secondsUntilTarget);
            hasScheduled = true;
        }

        if (scheduledTimes == null || scheduledTimes.Count == 0)
            return;

        while (scheduledTimes.Count > 0 && secondsUntilTarget <= scheduledTimes[0])
        {
            scheduledTimes.RemoveAt(0);

            // 셧다운이 켜져 있는 동안 도달한 팝업은 건너뜀 (몰아서 뜨지 않도록 버림)
            if (MainGameManager.GetInstance().IsShutdownActive)
            {
                Debug.Log("[DistractionPopupSpawner] 셧다운 중이라 팝업을 건너뜁니다.");
                continue;
            }

            SpawnPopup();
        }
    }

    private void GenerateSchedule(float secondsUntilTargetAtStart)
    {
        float earliest = secondsUntilTargetAtStart - startDelayAfterMainGameStart; // 가장 이른 시점
        float latest = endSecondsBeforeTarget;                                     // 가장 늦은 시점

        if (earliest <= latest)
        {
            Debug.LogWarning($"[DistractionPopupSpawner] 팝업을 띄울 구간이 없습니다. (시작 시 남은 시간 {secondsUntilTargetAtStart:F1}초, 종료 기준 {endSecondsBeforeTarget}초 전)");
            scheduledTimes = new List<float>();
            return;
        }

        int count = Random.Range(minPopupCount, maxPopupCount + 1);
        scheduledTimes = new List<float>();

        for (int i = 0; i < count; i++)
            scheduledTimes.Add(Random.Range(latest, earliest));

        scheduledTimes = scheduledTimes.OrderByDescending(time => time).ToList();

        Debug.Log($"[DistractionPopupSpawner] 팝업 {count}회 스케줄링됨 (남은 시간 {earliest:F1}초 ~ {latest:F1}초 사이)");
    }

    private void SpawnPopup()
    {
        if (popupPrefab == null || popupParent == null)
        {
            Debug.LogWarning("[DistractionPopupSpawner] popupPrefab 또는 popupParent 미연결");
            return;
        }

        GameObject popup = Instantiate(popupPrefab, popupParent);

        RectTransform rectTransform = popup.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            float x = Random.Range(randomPosMin.x, randomPosMax.x);
            float y = Random.Range(randomPosMin.y, randomPosMax.y);
            rectTransform.anchoredPosition = new Vector2(x, y);
        }

        SFXManager.GetInstance().PlaySFX(popupSfxName);
    }
}