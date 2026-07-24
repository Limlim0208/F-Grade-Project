using System.Collections;
using TMPro;
using UnityEngine;

public class ServerWaitingPopup : MonoBehaviour
{
    [SerializeField]
    private TMP_Text waitTimeText;
    [SerializeField]
    private TMP_Text remainingPeopleText;

    private const int MinWaitSeconds = 5;
    private const int MaxWaitSeconds = 20;
    private const int MinPeopleCount = 100;
    private const int MaxPeopleCount = 1500;

    private Coroutine waitingRoutine;

    private void OnEnable()
    {
        waitingRoutine = StartCoroutine(WaitingRoutine());
    }

    private void OnDisable()
    {
        if (waitingRoutine != null)
        {
            StopCoroutine(waitingRoutine);
            waitingRoutine = null;
        }
    }

    private IEnumerator WaitingRoutine()
    {
        int waitSeconds = Random.Range(MinWaitSeconds, MaxWaitSeconds + 1);

        // 대기시간이 길수록 남은 인원도 많아지도록 비례한 기준값을 잡고, 자연스럽게 보이도록 약간의 편차만 준다
        float t = (float)(waitSeconds - MinWaitSeconds) / (MaxWaitSeconds - MinWaitSeconds);
        int baseCount = Mathf.RoundToInt(Mathf.Lerp(MinPeopleCount, MaxPeopleCount, t));
        int variance = Mathf.RoundToInt(baseCount * 0.15f);
        int remainingCount = Mathf.Clamp(baseCount + Random.Range(-variance, variance + 1), MinPeopleCount, MaxPeopleCount);

        int secondsRemaining = waitSeconds;
        UpdateTexts(secondsRemaining, remainingCount);

        while (secondsRemaining > 0)
        {
            yield return new WaitForSeconds(1f);
            secondsRemaining--;

            if (secondsRemaining == 0)
            {
                remainingCount = 0;
            }
            else
            {
                int avg = Mathf.Max(1, remainingCount / (secondsRemaining + 1));
                int spread = Mathf.Max(1, avg / 2);
                int decrement = Random.Range(Mathf.Max(1, avg - spread), avg + spread + 1);
                decrement = Mathf.Clamp(decrement, 0, Mathf.Max(0, remainingCount - 1));
                remainingCount -= decrement;
            }

            UpdateTexts(secondsRemaining, remainingCount);
        }

        waitingRoutine = null;
        gameObject.SetActive(false);
    }

    private void UpdateTexts(int seconds, int peopleCount)
    {
        if (waitTimeText != null)
            waitTimeText.text = $"예상 대기시간 {seconds}초";

        if (remainingPeopleText != null)
            remainingPeopleText.text = $"사용자 앞에 <color=#2339C8>{peopleCount}</color>명의 대기자가 있습니다\n현재 접속 사용자가 많아 대기 중이며, 잠시만 기다리시면\n서비스로 자동 접속됩니다";
    }
}
