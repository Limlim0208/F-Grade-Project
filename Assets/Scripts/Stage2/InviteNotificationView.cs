using TMPro;
using UnityEngine;

// "OOO님이 초대했습니다" 알림 프리팹에 붙이는 컴포넌트
public class InviteNotificationView : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float maxTextWidth = 450f; // Layout Group 설정에 기대지 않고 코드로 직접 폭 고정

    public void SetText(string text)
    {
        if (messageText == null) return;

        messageText.text = text;

        // 폭을 먼저 고정한 다음, 그 폭 기준으로 줄바꿈된 실제 높이를 계산해서 반영
        var rt = messageText.rectTransform;
        rt.sizeDelta = new Vector2(maxTextWidth, rt.sizeDelta.y);
        messageText.ForceMeshUpdate();
        rt.sizeDelta = new Vector2(maxTextWidth, messageText.preferredHeight);
    }
}
