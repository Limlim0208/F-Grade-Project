using UnityEngine;
using System.Collections;

public class TooltipBubble : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup; // 페이드 처리용
    public float displayTime = 2f;  // 말풍선 표시 시간
    public float fadeTime = 1f;     // 페이드아웃 시간

    public void Show()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 1f;
        gameObject.SetActive(true);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        // displayTime 동안 대기
        yield return new WaitForSeconds(displayTime);

        // fadeTime 동안 서서히 사라짐
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsed / fadeTime);
            yield return null;
        }

        gameObject.SetActive(false);
        canvasGroup.alpha = 1f; // 다음 표시를 위해 초기화
    }
}