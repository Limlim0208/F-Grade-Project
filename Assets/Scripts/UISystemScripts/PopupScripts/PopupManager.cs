using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    // 팝업 프리팹
    [SerializeField]
    private Popup popupPrefab;
    // 팝업이 나타났을때 어둡게 만들어줄 배경 이미지
    [SerializeField]
    private Image darkBg;

    // 버튼타입에대한 정보를 저장할 딕셔너리 에셋을 사용해서 인스펙터에서 보이게 만든 후 세팅
    [SerializeField]
    private List<PopupButtonInfoEntry> buttonInfoList;
    private Dictionary<Enums.PopupButtonType, PopupButtonInfo> buttonInfoDict;

    // 팝업을 재사용할 풀 리스트
    private List<Popup> popupPool = new List<Popup>();
    // 현재 켜진 팝업
    private Popup currentActivePopup;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        BuildButtonInfoDict();
    }

    private void BuildButtonInfoDict()
    {
        buttonInfoDict = new Dictionary<Enums.PopupButtonType, PopupButtonInfo>();

        if (buttonInfoList == null)
            return;

        foreach (var entry in buttonInfoList)
        {
            if (buttonInfoDict.ContainsKey(entry.ButtonType))
            {
                Debug.LogWarning($"PopupManager: 중복된 ButtonType이 있습니다 - {entry.ButtonType}");
                continue;
            }

            buttonInfoDict.Add(entry.ButtonType, entry.Info);
        }
    }

    // 테스트용 팝업 코드
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var info = new PopupInfo.Builder().SetContent("테스트 입니다.")
                .SetButtons(Enums.PopupButtonType.Close, Enums.PopupButtonType.Confirm)
                .SetListener((type) =>
                {
                    switch (type)
                    {
                        case Enums.PopupButtonType.Close:
                            currentActivePopup.Hide();
                            break;
                        case Enums.PopupButtonType.Confirm:
                            currentActivePopup.Hide();
                            break;
                    }
                })
                .Build();

            ShowPopup(info);
        }
    }

    // 팝업 띄우기
    public void ShowPopup(PopupInfo info)
    {
        // TODO: 다중으로 띄워야 한다면 고치기 => currentActivePopup을 List로
        if (currentActivePopup != null)
            return;

        Popup popup = null;
        foreach (var pop in popupPool)
        {
            if (!pop.IsShow)
            {
                popup = pop;
                break;
            }
        }

        if (popup == null)
        {
            popup = Instantiate(popupPrefab, transform);
            popupPool.Add(popup);
        }

        popup.Init(info);
        popup.Show();
        currentActivePopup = popup;

        darkBg.enabled = true;
    }

    // 딕셔너리에서 value받아오기 (외부에서 호출 용도)
    public PopupButtonInfo GetButtonInfo(Enums.PopupButtonType type)
    {
        if (!buttonInfoDict.ContainsKey(type))
            return null;

        return buttonInfoDict[type];
    }

    // 팝업이 닫혔을 때 실행 (Popup클래스의 Hide()함수와 연결돼있다)
    public void OnPopupClose(Popup pop)
    {
        darkBg.enabled = false;

        if (pop == currentActivePopup)
            currentActivePopup = null;
    }

    // 외부 클래스에서 현재 활성화된 팝업을 끄기위해 호출
    public void CloseCurrentActivePopup()
    {
        if (currentActivePopup == null)
            return;

        currentActivePopup.Hide();
    }
}

/* 
* 버튼 정보를 저장할 클래스
* 더 확장하면 버튼 컬러나 버튼의 Sprite등을 저장해두면 된다
*/
[Serializable]
public class PopupButtonInfo
{
    [SerializeField]
    private string buttonText;

    public string ButtonText { get => buttonText; }
}

[Serializable]
public class PopupButtonInfoEntry
{
    [SerializeField]
    private Enums.PopupButtonType buttonType;
    [SerializeField]
    private PopupButtonInfo info;

    public Enums.PopupButtonType ButtonType { get => buttonType; }
    public PopupButtonInfo Info { get => info; }
}