using System.Collections.Generic;

[System.Serializable]
public class ButtonData
{
    public string label;
    public string type; // null이면 일반 이동, "gameOver" / "patternClear" 등 특수 동작에만 사용
    public int nextId;
}

[System.Serializable]
public class DialogueEntry
{
    public int id;
    public string text;
    public List<string> textPool;   // 랜덤 텍스트 풀 (있으면 랜덤 pick, 없으면 text 사용)
    public string contentType; // 생략 시 "question" 동작 / "scroll" / "input"
    public List<ButtonData> buttons;
}

[System.Serializable]
public class DialogueDatabase
{
    public List<DialogueEntry> dialogues;
}