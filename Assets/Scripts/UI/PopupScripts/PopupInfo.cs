using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PopupInfo
{
    public string Title { get; private set; }
    public string Content { get; private set; }
    public bool PauseScene { get; private set; }
    public Enums.PopupButtonType[] ButtonTypes { get; private set; }
    public Action<Enums.PopupButtonType> Listener { get; private set; }


    // === input 타입 ===
    public bool HasInput { get; private set; }
    public string InputPlaceholder { get; private set; }
    public string InputDefaultValue { get; private set; }
    public int InputMaxLength { get; private set; }

    public bool HasProfileOptions { get; private set; }

    private PopupInfo(Builder builder)
    {
        Title = builder.Title;
        Content = builder.Content;
        PauseScene = builder.PauseScene;
        ButtonTypes = builder.ButtonTypes;
        Listener = builder.Listener;

        // === inputtype ===
        HasInput = builder.HasInput;
        InputPlaceholder = builder.InputPlaceholder;
        InputDefaultValue = builder.InputDefaultValue;
        InputMaxLength = builder.InputMaxLength;

        HasProfileOptions = builder.HasProfileOptions;
    }

    // 하위 Builder클래스
    public class Builder
    {
        public string Title { get; private set; }
        public string Content { get; private set; }
        public bool PauseScene { get; private set; }
        public Enums.PopupButtonType[] ButtonTypes { get; private set; }
        public Action<Enums.PopupButtonType> Listener { get; private set; }

        // === input type ===
        public bool HasInput { get; private set; }
        public string InputPlaceholder { get; private set; }
        public string InputDefaultValue { get; private set; }
        public int InputMaxLength { get; private set; }

        public bool HasProfileOptions { get; private set; }
        public Builder()
        {
            Title = string.Empty;
            Content = string.Empty;
            ButtonTypes = null;
            Listener = null;
            PauseScene = false;

            // === input type ===
            HasInput = false;
            InputPlaceholder = string.Empty;
            InputDefaultValue = string.Empty;
            InputMaxLength = 0;

            HasProfileOptions = false;
        }

        // 제목 세팅
        public Builder SetTitle(string title)
        {
            this.Title = title;
            return this;
        }

        // 컨텐츠(본문 내용) 세팅
        public Builder SetContent(string content)
        {
            this.Content = content;
            return this;
        }

        // 어떤 버튼들이 들어갈지 세팅
        public Builder SetButtons(params Enums.PopupButtonType[] buttonTypes)
        {
            this.ButtonTypes = buttonTypes;
            return this;
        }

        // 콜백 세팅
        public Builder SetListener(Action<Enums.PopupButtonType> listener)
        {
            this.Listener = listener;
            return this;
        }

        // 팝업이 켜졌을때 시간을 멈출지 세팅
        public Builder SetPauseScene(bool isPause)
        {
            this.PauseScene = isPause;
            return this;
        }
        
        //텍스트 입력 팝업
        public Builder SetInput(string placeholder = "", string defaultValue = "", int maxLength = 0)
        {
            this.HasInput = true;
            this.InputPlaceholder = placeholder;
            this.InputDefaultValue = defaultValue;
            this.InputMaxLength = maxLength;
            return this;
        }

        public Builder SetProfileOptions()
        {
            this.HasProfileOptions = true;
            return this;
        }

        // 최종 빌드
        public PopupInfo Build()
        {
            return new PopupInfo(this);
        }
    }
}