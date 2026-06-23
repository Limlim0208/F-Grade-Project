using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UnityEngine.UI.Button 컴포넌트를 상속
public class PopupButton : Button
{
    [SerializeField]
    private Text buttonText;
    [SerializeField]
    private Enums.PopupButtonType buttonType;

    public Enums.PopupButtonType ButtonType { get => buttonType; }
    public Popup ParentPopup { get; set; }

    // 초기화 함수 => 버튼에 들어갈 String, 버튼 타입, 부모 팝업을 변수로 받는다
    public void Init(string buttonStr, Enums.PopupButtonType buttonType, Popup parentPopup)
    {
        if (buttonText == null)
            buttonText = GetComponentInChildren<Text>();

        buttonText.text = buttonStr;
        this.buttonType = buttonType;
        this.ParentPopup = parentPopup;
    }

    // 버튼이 클릭됐을때 실행된다.
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        ParentPopup.OnButtonClicked(this);
    }
}