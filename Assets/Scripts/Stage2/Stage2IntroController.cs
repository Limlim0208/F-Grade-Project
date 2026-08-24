using System.Collections.Generic;
using UnityEngine;

// Stage2 Day1 인트로 데모용 컨트롤러
// 초대 알림 -> 자기소개 -> 개발중 팝업 -> StartScene 이동 (선택지는 시간 관계상 생략)
public class Stage2IntroController : MonoBehaviour
{
    [SerializeField] private ChatManager chatManager;
    [SerializeField] private DateDividerView dayBadge; // 화면에 고정으로 떠있는 "DAY 1" 배지 (채팅 스크롤과 무관)

    void Start()
    {
        BGMManager.GetInstance().PlayBGM("stage2");

        dayBadge?.SetDate("DAY 1");
        dayBadge?.SetDeadline(6);
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
                text = "안녕하세요!! B입니다. 같이 팀플하게 돼서 너무 다행이에요! 잘 부탁드려요ㅎㅎ"
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
