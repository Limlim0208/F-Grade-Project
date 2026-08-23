using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [SerializeField]
    protected TMP_Text titleText;
    [SerializeField]
    protected TMP_Text contentText;
    [SerializeField]
    protected Transform buttonParent;
    [SerializeField]
    protected PopupButton buttonPrefab;
    [SerializeField]
    protected TMP_InputField inputField;

    protected Action<Enums.PopupButtonType> buttonClickedAction;
    protected List<PopupButton> myButtons = new List<PopupButton>();

    public bool IsShow => gameObject.activeSelf;


    public void Init(PopupInfo info)
    {
        // 제목 세팅
        titleText.text = info.Title;
        // 컨텐츠 세팅
        bool hasContent = !string.IsNullOrEmpty(info.Content);
        contentText.gameObject.SetActive(hasContent);
        if (hasContent)
            contentText.text = info.Content;
        // 콜백 세팅
        buttonClickedAction = info.Listener;
        // 버튼 세팅
        SetButtons(info.ButtonTypes);
        // 인풋 필드 세팅
        SetInput(info);
    }

    protected virtual void SetInput(PopupInfo info)
    {
        if (inputField == null)
            return;

        if (!info.HasInput)
        {
            inputField.gameObject.SetActive(false);
            return;
        }

        inputField.gameObject.SetActive(true);
        inputField.text = info.InputDefaultValue ?? string.Empty;

        if (inputField.placeholder is TMP_Text placeholderText)
            placeholderText.text = info.InputPlaceholder ?? string.Empty;

        inputField.characterLimit = info.InputMaxLength;
    }

    public string GetInputText()
    {
        return inputField != null ? inputField.text : null;
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