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
    private RectTransform inviteRectTransform; // Invite만 스크롤뷰 정중앙에 고정 보정하기 위해 기억해둠

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
            ChatBubbleView lastBubble = null; // 이번 루프에서 만든 말풍선(프사 정렬용) - 다음 루프에 안 넘어가게 매번 초기화

            switch (item.itemType)
            {
                case ChatItemType.DateDivider:
                    var divider = Instantiate(dateDividerPrefab, contentParent);
                    divider.SetDate(item.dateLabel);
                    previousMessageItem = null; // 날짜 구분자 다음엔 항상 꼬리 있는 말풍선부터 새로 시작
                    break;

                case ChatItemType.Invite:
                    var invite = Instantiate(inviteNotificationPrefab, contentParent);
                    invite.gameObject.SetActive(true); // 원본 프리팹이 꺼져있어도 복제본은 항상 켜지도록
                    invite.SetText(item.text);
                    inviteRectTransform = invite.transform as RectTransform;
                    previousMessageItem = null;
                    break;

                case ChatItemType.Choice:
                    yield return PlayChoice(item);
                    continue; // 선택지 처리(대기 + 반응 재생)는 자체적으로 스크롤/딜레이 처리하므로 아래 공통 처리 건너뜀

                default: // Message
                    bool isConsecutive = IsConsecutive(item);
                    ChatBubbleView prefab = GetBubblePrefab(item, isConsecutive);
                    var bubble = Instantiate(prefab, contentParent);
                    lastBubble = bubble;

                    if (item.contentType == BubbleContentType.Attachment)
                        bubble.SetAttachment(item.attachmentImage);
                    else
                        bubble.SetText(item.text);

                    // 상대방이고, 연속 메시지가 아닐 때(꼬리 있는 첫 말풍선)만 프로필 사진 + 이름 표시
                    if (item.speaker == ChatSpeaker.Other && !isConsecutive)
                    {
                        bubble.SetProfile(FindPortrait(item.speakerId));
                        bubble.SetName(item.speakerId);
                    }

                    UpdatePortrait(item);
                    previousMessageItem = item;
                    break;
            }

            yield return null; // 레이아웃 갱신 한 프레임 대기
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent); // 말풍선 배경이 줄바꿈된 텍스트 크기에 안 맞는 문제 방지 (강제 즉시 재계산)
            RecenterInvite(); // 위 레이아웃 재계산 때마다 Content의 Padding Left 편향 때문에 Invite가 오른쪽으로 튕겨나가는 것 보정
            lastBubble?.AlignProfileToBackground(); // 배경 크기가 실제로 반영된 다음에 계산해야 프사가 정확히 배경 꼭대기에 맞음
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

    private bool IsConsecutive(ChatItem item)
    {
        return previousMessageItem != null
            && previousMessageItem.speaker == item.speaker
            && previousMessageItem.speakerId == item.speakerId
            && previousMessageItem.contentType == BubbleContentType.Text
            && item.contentType == BubbleContentType.Text;
    }

    private ChatBubbleView GetBubblePrefab(ChatItem item, bool isConsecutive)
    {
        if (item.contentType == BubbleContentType.Attachment)
            return item.speaker == ChatSpeaker.Player ? playerAttachmentBubble : otherAttachmentBubble;

        if (item.speaker == ChatSpeaker.Player)
            return isConsecutive ? playerTextNoTailBubble : playerTextTailBubble;
        else
            return isConsecutive ? otherTextNoTailBubble : otherTextTailBubble;
    }

    private Sprite FindPortrait(string speakerId)
    {
        var match = characterPortraits.Find(p => p.speakerId == speakerId);
        return match?.portrait;
    }

    private void UpdatePortrait(ChatItem item)
    {
        // 플레이어가 말할 땐 직전 상대방 초상화를 그대로 유지
        if (item.speaker != ChatSpeaker.Other) return;

        if (portraitImage != null)
        {
            var sprite = FindPortrait(item.speakerId);
            if (sprite != null)
                portraitImage.sprite = sprite;
        }

        groupIllustration?.Reveal(item.speakerId); // Day1 그룹 일러스트에 쓰는 경우 실루엣 자동 리빌
    }

    // Content의 Vertical Layout Group이 Padding Left/Right가 비대칭(초상화 자리 확보용)이라
    // ChildAlignment=Center여도 말풍선들은 스크롤뷰 진짜 중앙이 아니라 그 패딩만큼 치우쳐서 배치됨.
    // 패딩값으로 보정폭을 역산하는 대신, Content 폭의 정중앙으로 직접 못박음 (pivot 0.5 기준이라 이 값이 바로 중앙)
    private void RecenterInvite()
    {
        if (inviteRectTransform == null) return;

        var pos = inviteRectTransform.anchoredPosition;
        pos.x = contentParent.rect.width / 2f;
        inviteRectTransform.anchoredPosition = pos;
    }

    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
