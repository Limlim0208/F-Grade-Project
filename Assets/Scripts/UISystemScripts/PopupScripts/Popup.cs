using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Popup : MonoBehaviour
{
    [SerializeField]
    protected Text contentText;
    [SerializeField]
    protected Transform buttonParent;
    [SerializeField]
    protected PopupButton buttonPrefab;

    protected Action<Enums.PopupButtonType> buttonClickedAction;
    protected List<PopupButton> myButtons = new List<PopupButton>();

    public bool IsShow => gameObject.activeSelf;


    public void Init(PopupInfo info)
    {
        // 컨텐츠 세팅
        contentText.text = info.Content;
        // 콜백 세팅
        buttonClickedAction = info.Listener;
        // 버튼 세팅
        SetButtons(info.ButtonTypes);
    }

    protected virtual void SetButtons(Enums.PopupButtonType[] buttonTypes)
    {
        myButtons = new List<PopupButton>();
        for (int i = 0; i < buttonTypes.Length; i++)
        {
            // 버튼 생성
            var clone = Instantiate(buttonPrefab, buttonParent);
            var info = PopupManager.Instance.GetButtonInfo(buttonTypes[i]);
            clone.Init(info.ButtonText, buttonTypes[i], this);

            myButtons.Add(clone);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }


    public void Hide()
    {
        foreach (var btn in myButtons)
        {
            Destroy(btn.gameObject);
        }

        PopupManager.Instance.OnPopupClose(this);

        gameObject.SetActive(false);
    }

    public void OnButtonClicked(PopupButton btn)
    {
        // 콜백 실행
        buttonClickedAction?.Invoke(btn.ButtonType);
    }
}