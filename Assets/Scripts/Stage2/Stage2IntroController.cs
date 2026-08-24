using System.Collections.Generic;
using UnityEngine;

// Stage2 Day1 인트로 데모용 컨트롤러
// 초대 알림 -> 자기소개 -> 선택지 -> 반응 -> 개발중 팝업 -> StartScene 이동
public class Stage2IntroController : MonoBehaviour
{
    [SerializeField] private ChatManager chatManager;

    void Start()
    {
        chatManager.PlayChat(BuildDay1Items(), ShowDevPopup);
    }

    private List<ChatItem> BuildDay1Items()
    {
        string playerName = PlayerDataManager.GetInstance().PlayerName;

        var items = new List<ChatItem>
        {
            new ChatItem
            {
                itemType = ChatItemType.Invite,
                text = $"<u>{playerName}</u>님이 <u>A</u>님, <u>B</u>님, <u>C</u>님을 초대했습니다."
            },
            new ChatItem
            {
                itemType = ChatItemType.DateDivider,
                dateLabel = "DAY 1"
            },
            new ChatItem
            {
                speaker = ChatSpeaker.Other,
                speakerId = "C",
                text = "안녕하세요, C입니다. 이전에도 팀플을 많이 해봐서 역할 분담이 가장 중요하다고 생각합니다."
            },
            new ChatItem
            {
                speaker = ChatSpeaker.Other,
                speakerId = "A",
                text = "팀플 같이하게 된 A입니다!"
            },
            new ChatItem
            {
                speaker = ChatSpeaker.Other,
                speakerId = "B",
                text = "안녕하세요!! B입니다"
            },
            new ChatItem
            {
                speaker = ChatSpeaker.Other,
                speakerId = "B",
                text = "같이 팀플하게 돼서 너무 다행이에요! 잘 부탁드려요ㅎㅎ"
            },
            new ChatItem
            {
                itemType = ChatItemType.Choice,
                choices = new List<ChatChoice>
                {
                    new ChatChoice
                    {
                        choiceText = "다들 원하시는 역할 있으신가요?",
                        reactionItems = new List<ChatItem>
                        {
                            new ChatItem { speaker = ChatSpeaker.Player, text = "다들 원하시는 역할 있으신가요?" },
                            new ChatItem { speaker = ChatSpeaker.Other, speakerId = "C", text = "저는 자료조사할게요" },
                            new ChatItem { speaker = ChatSpeaker.Other, speakerId = "A", text = "제가 PPT 만들게요" },
                        }
                    },
                    new ChatChoice
                    {
                        choiceText = "C님이 PPT, A님이 자료조사, B님이 최종 정리 맡아주세요..!",
                        reactionItems = new List<ChatItem>
                        {
                            new ChatItem { speaker = ChatSpeaker.Player, text = "C님이 PPT, A님이 자료조사, B님이 최종 정리 맡아주세요..!" },
                            new ChatItem { speaker = ChatSpeaker.Other, speakerId = "C", text = "네 알겠습니다" },
                            new ChatItem { speaker = ChatSpeaker.Other, speakerId = "A", text = "넵 ㅎㅎ" },
                        }
                    }
                }
            }
        };

        return items;
    }

    private void ShowDevPopup()
    {
        var info = new PopupInfo.Builder()
            .SetTitle("")
            .SetContent("앗 아직 개발중입니다 ㅎㅎ;;")
            .SetButtons(Enums.PopupButtonType.Confirm)
            .SetListener((type) =>
            {
                if (type == Enums.PopupButtonType.Confirm)
                {
                    PopupManager.Instance.CloseCurrentActivePopup();
                    SceneChanger.GetInstance().LoadScene("StartScene");
                }
            })
            .Build();

        PopupManager.Instance.ShowPopup(info);
    }
}
