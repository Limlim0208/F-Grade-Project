using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DistractionPopupSpawner : MonoBehaviour
{
    [Header("팝업 프리팹 / 부모(Canvas)")]
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Transform popupParent;

    [Header("발생 구간 (9시까지 남은 시간 기준, 초)")]
    [SerializeField] private float windowStartSecondsBeforeTarget = 120f;
    [SerializeField] private float windowEndSecondsBeforeTarget = 30f;

    [Header("발생 횟수 (구간당 랜덤)")]
    [SerializeField] private int minPopupCount = 7;
    [SerializeField] private int maxPopupCount = 10;

    [Header("팝업 스폰 위치 랜덤 범위 (anchoredPosition 기준)")]
    [SerializeField] private Vector2 randomPosMin = new Vector2(-300f, -200f);
    [SerializeField] private Vector2 randomPosMax = new Vector2(300f, 200f);

    // "9시까지 남은 시간" 기준 트리거 시점들 (내림차순: 큰 값 = 더 이른 시점)
    private List<float> scheduledTimes;
    private bool hasScheduled = false;

    void Start()
    {
        ServerClockManager.GetInstance().OnTick += HandleTick;
    }

    void OnDisable()
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

        // 구간(120초~30초) 진입 시점에 딱 한 번만 스케줄 생성
        if (!hasScheduled && secondsUntilTarget <= windowStartSecondsBeforeTarget && secondsUntilTarget > windowEndSecondsBeforeTarget)
        {
            GenerateSchedule();
            hasScheduled = true;
        }

        if (scheduledTimes == null || scheduledTimes.Count == 0)
            return;

        // 시간이 흐르며 남은 시간이 예약된 시점 이하로 내려갈 때마다 발동
        while (scheduledTimes.Count > 0 && secondsUntilTarget <= scheduledTimes[0])
        {
            scheduledTimes.RemoveAt(0);
            SpawnPopup();
        }
    }

    private void GenerateSchedule()
    {
        int count = Random.Range(minPopupCount, maxPopupCount + 1);
        scheduledTimes = new List<float>();

        for (int i = 0; i < count; i++)
            scheduledTimes.Add(Random.Range(windowEndSecondsBeforeTarget, windowStartSecondsBeforeTarget));

        // 남은시간이 큰 값(이른 시점)부터 트리거되도록 내림차순 정렬
        scheduledTimes = scheduledTimes.OrderByDescending(t => t).ToList();

        Debug.Log($"[DistractionPopupSpawner] 팝업 {count}회 스케줄링됨");
    }

    private void SpawnPopup()
    {
        if (popupPrefab == null || popupParent == null)
        {
            Debug.LogWarning("[DistractionPopupSpawner] popupPrefab 또는 popupParent 미연결");
            return;
        }

        GameObject popup = Instantiate(popupPrefab, popupParent);

        RectTransform rt = popup.GetComponent<RectTransform>();
        if (rt != null)
        {
            float x = Random.Range(randomPosMin.x, randomPosMax.x);
            float y = Random.Range(randomPosMin.y, randomPosMax.y);
            rt.anchoredPosition = new Vector2(x, y);
        }
    }
}