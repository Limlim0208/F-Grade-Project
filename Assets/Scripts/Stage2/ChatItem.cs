using UnityEngine;

public enum ChatSpeaker { Player, Other }
public enum BubbleContentType { Text, Attachment }
public enum ChatItemType { Message, DateDivider }

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

    [Header("DateDivider용")]
    public string dateLabel; // 예: "2026년 8월 5일" 또는 "DAY 1"

    [Header("공통")]
    public float delayAfter = 1f; // 이 메시지가 뜬 후 다음 메시지까지 대기시간
}
