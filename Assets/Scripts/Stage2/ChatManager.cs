using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [Header("스크롤 영역")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentParent;

    [Header("플레이어 말풍선")]
    [SerializeField] private ChatBubbleView playerTextTailBubble;
    [SerializeField] private ChatBubbleView playerTextNoTailBubble;
    [SerializeField] private ChatBubbleView playerAttachmentBubble;

    [Header("상대방 말풍선")]
    [SerializeField] private ChatBubbleView otherTextTailBubble;
    [SerializeField] private ChatBubbleView otherTextNoTailBubble;
    [SerializeField] private ChatBubbleView otherAttachmentBubble;

    [Header("날짜 구분자")]
    [SerializeField] private DateDividerView dateDividerPrefab;

    [System.Serializable]
    public class CharacterPortrait
    {
        public string speakerId; // ChatItem.speakerId와 매칭 (예: "A", "B", "C")
        public Sprite portrait;
    }

    [Header("캐릭터 초상화 (왼쪽)")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private List<CharacterPortrait> characterPortraits;

    private Coroutine playRoutine;
    private ChatItem previousMessageItem; // 연속 메시지(꼬리 없음) 판단용

    public void PlayChat(List<ChatItem> items)
    {
        if (playRoutine != null)
            StopCoroutine(playRoutine);

        previousMessageItem = null;
        playRoutine = StartCoroutine(PlayChatRoutine(items));
    }

    public void StopChat()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }
    }

    private IEnumerator PlayChatRoutine(List<ChatItem> items)
    {
        foreach (var item in items)
        {
            if (item.itemType == ChatItemType.DateDivider)
            {
                var divider = Instantiate(dateDividerPrefab, contentParent);
                divider.SetDate(item.dateLabel);
                previousMessageItem = null; // 날짜 구분자 다음엔 항상 꼬리 있는 말풍선부터 새로 시작
            }
            else
            {
                ChatBubbleView prefab = GetBubblePrefab(item);
                var bubble = Instantiate(prefab, contentParent);

                if (item.contentType == BubbleContentType.Attachment)
                    bubble.SetAttachment(item.attachmentImage);
                else
                    bubble.SetText(item.text);

                UpdatePortrait(item);
                previousMessageItem = item;
            }

            yield return null; // 레이아웃 갱신 한 프레임 대기
            ScrollToBottom();

            yield return new WaitForSeconds(item.delayAfter);
        }

        playRoutine = null;
    }

    private ChatBubbleView GetBubblePrefab(ChatItem item)
    {
        bool isConsecutive = previousMessageItem != null
            && previousMessageItem.speaker == item.speaker
            && previousMessageItem.contentType == BubbleContentType.Text
            && item.contentType == BubbleContentType.Text;

        if (item.contentType == BubbleContentType.Attachment)
            return item.speaker == ChatSpeaker.Player ? playerAttachmentBubble : otherAttachmentBubble;

        if (item.speaker == ChatSpeaker.Player)
            return isConsecutive ? playerTextNoTailBubble : playerTextTailBubble;
        else
            return isConsecutive ? otherTextNoTailBubble : otherTextTailBubble;
    }

    private void UpdatePortrait(ChatItem item)
    {
        // 플레이어가 말할 땐 직전 상대방 초상화를 그대로 유지
        if (item.speaker != ChatSpeaker.Other || portraitImage == null) return;

        var match = characterPortraits.Find(p => p.speakerId == item.speakerId);
        if (match != null)
            portraitImage.sprite = match.portrait;
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
