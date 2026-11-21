using UnityEngine;

public class SubjectRegisterButton : MonoBehaviour
{
    [SerializeField] private float closeOffsetSeconds; // 이 과목이 마감되는, 목표시각 기준 초 (ex. B과목은 1초 후 마감)

    public void OnClickRegister()
    {
        var clock = ServerClockManager.GetInstance();

        if (!clock.IsRegistrationOpen)
        {
            // 09:00:00 이전: 신청 불가 알림
            //ShowNotYetOpenNotice();
            return;
        }

        if (clock.SecondsSinceRegistrationOpened <= closeOffsetSeconds)
        {
            // 성공
            //RegisterSuccess();
        }
        else
        {
            // 실패 (마감됨)
            //RegisterFail();
        }
    }
}