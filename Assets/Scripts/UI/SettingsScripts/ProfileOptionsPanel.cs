using UnityEngine;
using UnityEngine.UI;

public class ProfileOptionsPanel : MonoBehaviour
{
    [SerializeField] private Toggle option0;
    [SerializeField] private Toggle option1;
    [SerializeField] private Toggle option2;

    private int selectedIndex;

    // 팝업이 열릴 때 현재 프로필 이미지 기준으로 초기 선택 상태 세팅
    public void Refresh()
    {
        selectedIndex = SettingsManager.Instance.ProfileImageIndex;

        option0.SetIsOnWithoutNotify(selectedIndex == 0);
        option1.SetIsOnWithoutNotify(selectedIndex == 1);
        option2.SetIsOnWithoutNotify(selectedIndex == 2);
    }

    // 각 Toggle의 OnValueChanged(bool)에 인스펙터에서 연결
    public void OnOption0Selected(bool isOn) { Debug.Log($"[Profile] Option0 isOn={isOn}"); if (isOn) selectedIndex = 0; }
    public void OnOption1Selected(bool isOn) { Debug.Log($"[Profile] Option1 isOn={isOn}"); if (isOn) selectedIndex = 1; }
    public void OnOption2Selected(bool isOn) { Debug.Log($"[Profile] Option2 isOn={isOn}"); if (isOn) selectedIndex = 2; }

    public int GetSelectedIndex() => selectedIndex;
}