using UnityEngine;

public class HomeBGMPlayer : MonoBehaviour
{
    void Start()
    {
        BGMManager.GetInstance().PlayBGM("main");
        SFXManager.GetInstance(); // 클릭 효과음 감지를 위해 여기서 미리 생성해둠
    }
}
