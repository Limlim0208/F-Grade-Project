using System.Collections.Generic;
using UnityEngine;

public enum ChatSpeaker { Player, Other }
public enum BubbleContentType { Text, Attachment }
public enum ChatItemType { Message, DateDivider, Invite, Choice }

[System.Serializable]
public class ChatChoice
{
    [TextArea] public string choiceText;
    public List<ChatItem> reactionItems; // 이 선택지를 고르면 이어서 재생될 대사들
}

[System.Serializable]
public class ChatItem
{
    public ChatItemType itemType = ChatItemType.Message;

    [Header("Message용")]
    public ChatSpeaker speaker;
    public string speakerId; // 상대방(Other)일 때 어느 캐릭터인지 (예: "A", "B", "C"). 초상화 매칭에 사용
    public BubbleContentType contentType = BubbleContentType.Text;
    [TextArea] public string text;
    public Sprite attachmentImage; // contentType이 Attachment일 때 사용

    [Header("DateDivider / Invite용")]
    public string dateLabel; // 예: "2026년 8월 5일" 또는 "DAY 1" (DateDivider용)
    // Invite용 텍스트는 위 text 필드를 그대로 사용

    [Header("Choice용")]
    [System.NonSerialized] public List<ChatChoice> choices; // ChatChoice <-> ChatItem 상호 참조라 Unity 직렬화 대상에서 제외 (코드로만 채워서 씀)

    [Header("공통")]
    public float delayAfter = 2f; // 이 메시지가 뜬 후 다음 메시지까지 대기시간
}
