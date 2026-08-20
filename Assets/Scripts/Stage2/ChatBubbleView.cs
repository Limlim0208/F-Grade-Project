using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 말풍선 프리팹(텍스트 꼬리O/꼬리X/첨부파일 6종)에 공통으로 붙이는 컴포넌트.
// 프리팹마다 안 쓰는 필드는 비워두면 됨 (예: 텍스트 전용 프리팹엔 attachmentImage 연결 안 해도 됨)
public class ChatBubbleView : MonoBehaviour
{
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Image attachmentImage;

    public void SetText(string text)
    {
        if (bodyText != null)
            bodyText.text = text;
    }

    public void SetAttachment(Sprite sprite)
    {
        if (attachmentImage == null) return;

        attachmentImage.sprite = sprite;
        attachmentImage.gameObject.SetActive(sprite != null);
    }
}
