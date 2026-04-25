using System.Collections.Generic;

[System.Serializable]
public class ButtonData
{
    public string label;
    public string type; // 타입 종류: "correct", "wrong", "patternClear", "gameOver", "fake"
}

[System.Serializable]
public class DialogueEntry
{
    public int id;
    public string text;
    public string contentType;
    public List<ButtonData> buttons;
}

[System.Serializable]
public class DialogueDatabase
{
    public List<DialogueEntry> dialogues;
}