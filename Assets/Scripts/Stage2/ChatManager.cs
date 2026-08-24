using System;
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

    [Header("초대 알림")]
    [SerializeField] private InviteNotificationView inviteNotificationPrefab;

    [Header("선택지")]
    [SerializeField] private ChoiceButtonView choiceButtonPrefab;
    [SerializeField] private Transform choiceContainer;

    [System.Serializable]
    public class CharacterPortrait
    {
        public string speakerId; // ChatItem.speakerId와 매칭 (예: "A", "B", "C")
        public Sprite portrait;
    }

    [Header("캐릭터 초상화 (왼쪽)")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private List<CharacterPortrait> characterPortraits;

    [Header("그룹 일러스트 (실루엣 -> 본모습, 선택)")]
    [SerializeField] private GroupIllustrationView groupIllustration;

    private Coroutine playRoutine;
    private ChatItem previousMessageItem; // 연속 메시지(꼬리 없음) 판단용

    public void PlayChat(List<ChatItem> items, Action onComplete = null)
    {
        if (playRoutine != null)
            StopCoroutine(playRoutine);

        previousMessageItem = null;
        playRoutine = StartCoroutine(PlayChatRoutine(items, onComplete));
    }

    public void StopChat()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }
    }

    private IEnumerator PlayChatRoutine(List<ChatItem> items, Action onComplete)
    {
        yield return PlayItems(items);

        playRoutine = null;
        onComplete?.Invoke();
    }

    private IEnumerator PlayItems(List<ChatItem> items)
    {
        foreach (var item in items)
        {
            switch (item.itemType)
            {
                case ChatItemType.DateDivider:
                    var divider = Instantiate(dateDividerPrefab, contentParent);
                    divider.SetDate(item.dateLabel);
                    previousMessageItem = null; // 날짜 구분자 다음엔 항상 꼬리 있는 말풍선부터 새로 시작
                    break;

                case ChatItemType.Invite:
                    var invite = Instantiate(inviteNotificationPrefab, contentParent);
                    invite.SetText(item.text);
                    previousMessageItem = null;
                    break;

                case ChatItemType.Choice:
                    yield return PlayChoice(item);
                    continue; // 선택지 처리(대기 + 반응 재생)는 자체적으로 스크롤/딜레이 처리하므로 아래 공통 처리 건너뜀

                default: // Message
                    ChatBubbleView prefab = GetBubblePrefab(item);
                    var bubble = Instantiate(prefab, contentParent);

                    if (item.contentType == BubbleContentType.Attachment)
                        bubble.SetAttachment(item.attachmentImage);
                    else
                        bubble.SetText(item.text);

                    UpdatePortrait(item);
                    previousMessageItem = item;
                    break;
            }

            yield return null; // 레이아웃 갱신 한 프레임 대기
            ScrollToBottom();

            yield return new WaitForSeconds(item.delayAfter);
        }
    }

    private IEnumerator PlayChoice(ChatItem item)
    {
        bool chosen = false;
        List<ChatItem> reactionItems = null;
        var spawnedButtons = new List<GameObject>();

        foreach (var choice in item.choices)
        {
            var btn = Instantiate(choiceButtonPrefab, choiceContainer);
            spawnedButtons.Add(btn.gameObject);

            var capturedChoice = choice;
            btn.Setup(capturedChoice.choiceText, () =>
            {
                if (chosen) return; // 중복 클릭 방지
                chosen = true;
                reactionItems = capturedChoice.reactionItems;
            });
        }

        yield return null;
        ScrollToBottom();

        yield return new WaitUntil(() => chosen);

        foreach (var obj in spawnedButtons)
            Destroy(obj);

        previousMessageItem = null; // 선택지 이후엔 항상 꼬리 있는 말풍선부터 새로 시작

        if (reactionItems != null)
            yield return PlayItems(reactionItems);
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
        if (item.speaker != ChatSpeaker.Other) return;

        if (portraitImage != null)
        {
            var match = characterPortraits.Find(p => p.speakerId == item.speakerId);
            if (match != null)
                portraitImage.sprite = match.portrait;
        }

        groupIllustration?.Reveal(item.speakerId); // Day1 그룹 일러스트에 쓰는 경우 실루엣 자동 리빌
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
