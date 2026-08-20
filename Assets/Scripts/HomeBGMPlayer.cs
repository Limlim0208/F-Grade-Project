using UnityEngine;

public class HomeBGMPlayer : MonoBehaviour
{
    void Start()
    {
        BGMManager.GetInstance().PlayBGM("homepage");
    }
}
