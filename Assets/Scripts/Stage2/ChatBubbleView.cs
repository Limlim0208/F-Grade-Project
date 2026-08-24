using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 말풍선 프리팹(텍스트 꼬리O/꼬리X/첨부파일 6종)에 공통으로 붙이는 컴포넌트.
// 프리팹마다 안 쓰는 필드는 비워두면 됨 (예: 텍스트 전용 프리팹엔 attachmentImage 연결 안 해도 됨)
public class ChatBubbleView : MonoBehaviour
{
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Image attachmentImage;
    [SerializeField] private Image profileImage; // 단톡방 스타일 프로필 사진 (꼬리 있는 상대방 말풍선에만 씀)
    [SerializeField] private TMP_Text nameLabel; // 프로필 옆 이름 (꼬리 있는 상대방 말풍선에만 씀)
    [SerializeField] private float maxTextWidth = 350f; // 말풍선 최대 폭 (Layout Group 설정에 기대지 않고 코드로 직접 고정)

    [SerializeField] private RectTransform chatBackground; // 배경 이미지(ChatImg) - 말풍선 크기가 유동적이라 프사 위치를 여기 실제 위쪽 끝에 맞추는 데 씀
    [SerializeField] private float profileTopMargin = 1.9f; // 배경 위쪽 끝에서 프사까지 살짝 띄우는 여백

    public void SetText(string text)
    {
        if (bodyText == null) return;

        bodyText.text = text;

        // 폭을 먼저 고정한 다음, 그 폭 기준으로 줄바꿈된 실제 높이를 계산해서 반영
        var rt = bodyText.rectTransform;
        rt.sizeDelta = new Vector2(maxTextWidth, rt.sizeDelta.y);
        bodyText.ForceMeshUpdate();
        rt.sizeDelta = new Vector2(maxTextWidth, bodyText.preferredHeight);
    }

    // 말풍선 폭/높이가 텍스트 길이에 따라 유동적으로 변하므로, SetText 이후 실제 레이아웃이
    // 반영된 시점(ChatManager의 ForceRebuildLayoutImmediate 직후)에 호출해야 정확하게 맞음
    public void AlignProfileToBackground()
    {
        if (profileImage == null || chatBackground == null) return;

        var profileRect = profileImage.rectTransform;
        float bgTop = chatBackground.anchoredPosition.y + chatBackground.rect.height * (1f - chatBackground.pivot.y);

        var pos = profileRect.anchoredPosition;
        pos.y = bgTop - profileTopMargin;
        profileRect.anchoredPosition = pos;
    }

    public void SetProfile(Sprite sprite)
    {
        if (profileImage == null) return;

        profileImage.sprite = sprite;
        profileImage.gameObject.SetActive(sprite != null);
    }

    public void SetName(string name)
    {
        if (nameLabel == null) return;

        nameLabel.text = name;
        nameLabel.gameObject.SetActive(!string.IsNullOrEmpty(name));
    }

    public void SetAttachment(Sprite sprite)
    {
        if (attachmentImage == null) return;

        attachmentImage.sprite = sprite;
        attachmentImage.gameObject.SetActive(sprite != null);
    }
}
