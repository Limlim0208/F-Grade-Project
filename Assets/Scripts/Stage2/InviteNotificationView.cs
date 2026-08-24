using TMPro;
using UnityEngine;

// "OOO님이 초대했습니다" 알림 프리팹에 붙이는 컴포넌트
public class InviteNotificationView : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;

    public void SetText(string text)
    {
        if (messageText != null)
            messageText.text = text;
    }
}
