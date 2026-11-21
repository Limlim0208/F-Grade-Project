using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewsTicker : MonoBehaviour
{
    [SerializeField] private RectTransform content;      // Text들의 부모
    [SerializeField] private List<string> messages;      // 안내 문구 목록
    [SerializeField] private TMP_Text textPrefab;        // 하나짜리 프리팹
    [SerializeField] private float itemHeight = 40f;     // 텍스트 한 줄 높이
    [SerializeField] private float holdTime = 2f;        // 한 문구 보여주는 시간
    [SerializeField] private float slideTime = 0.5f;     // 슬라이드 애니메이션 시간

    private Vector2 contentStartPosition;
    private List<TMP_Text> spawned = new List<TMP_Text>();

    private void Start()
    {
        // Content의 원래 위치를 저장해 둔다 (x가 0이 아닐 수 있음)
        contentStartPosition = content.anchoredPosition;

        for (int i = 0; i < messages.Count; i++)
        {
            TMP_Text text = Instantiate(textPrefab, content);
            text.text = messages[i];
            spawned.Add(text);
        }

        LayoutItems();

        // 문구가 1개 이하면 굴릴 필요 없음
        if (messages.Count > 1)
        {
            StartCoroutine(TickerLoop());
        }
    }

    // spawned 순서대로 세로 배치하고, Content를 원래 위치로 되돌린다
    private void LayoutItems()
    {
        for (int i = 0; i < spawned.Count; i++)
        {
            spawned[i].rectTransform.anchoredPosition = new Vector2(0f, -itemHeight * i);
        }

        content.anchoredPosition = contentStartPosition;
    }

    private IEnumerator TickerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(holdTime);

            Vector2 startPosition = content.anchoredPosition;
            Vector2 endPosition = startPosition + new Vector2(0f, itemHeight);

            float elapsed = 0f;
            while (elapsed < slideTime)
            {
                elapsed += Time.deltaTime;
                content.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsed / slideTime);
                yield return null;
            }
            content.anchoredPosition = endPosition;

            // 화면 밖으로 나간 맨 위 텍스트를 맨 뒤로 보내고 전체 재배치
            TMP_Text first = spawned[0];
            spawned.RemoveAt(0);
            spawned.Add(first);
            LayoutItems();
        }
    }
}