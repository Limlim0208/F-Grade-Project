using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewsTicker : MonoBehaviour
{
    [SerializeField] private RectTransform content;      // Text들의 부모
    [SerializeField] private List<string> messages;      // 안내 문구 목록
    [SerializeField] private TMP_Text textPrefab;         // 하나짜리 프리팹
    [SerializeField] private float itemHeight = 40f;      // 텍스트 한 줄 높이
    [SerializeField] private float holdTime = 2f;         // 한 문구 보여주는 시간
    [SerializeField] private float slideTime = 0.5f;      // 슬라이드 애니메이션 시간

    private int currentIndex = 0;
    private List<TMP_Text> spawned = new List<TMP_Text>();

    void Start()
    {
        // 문구들을 세로로 배치 (0번이 맨 위, 보이는 위치)
        for (int i = 0; i < messages.Count; i++)
        {
            var t = Instantiate(textPrefab, content);
            t.text = messages[i];
            t.rectTransform.anchoredPosition = new Vector2(0, -itemHeight * i);
            spawned.Add(t);
        }

        StartCoroutine(TickerLoop());
    }

    IEnumerator TickerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(holdTime);

            int nextIndex = (currentIndex + 1) % messages.Count;
            float targetY = content.anchoredPosition.y + itemHeight;

            float t = 0;
            Vector2 start = content.anchoredPosition;
            Vector2 end = new Vector2(start.x, targetY);

            while (t < slideTime)
            {
                t += Time.deltaTime;
                content.anchoredPosition = Vector2.Lerp(start, end, t / slideTime);
                yield return null;
            }
            content.anchoredPosition = end;

            currentIndex = nextIndex;

            // 마지막 문구였으면 맨 위 텍스트를 맨 아래로 재배치 (무한 루프처럼 보이게)
            if (currentIndex == 0)
            {
                var first = spawned[0];
                spawned.RemoveAt(0);
                spawned.Add(first);
                first.rectTransform.anchoredPosition = new Vector2(0, -itemHeight * (spawned.Count - 1));
                content.anchoredPosition = Vector2.zero;
            }
        }
    }
}